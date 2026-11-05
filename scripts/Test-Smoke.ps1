param(
    [Parameter(Mandatory)]
    [uri] $BaseUri,

    [Parameter(Mandatory)]
    [pscredential] $Credential
)

$ErrorActionPreference = 'Stop'
if (-not $BaseUri.IsAbsoluteUri -or -not $BaseUri.IsLoopback -or $BaseUri.Scheme -notin @('http', 'https')) {
    throw 'Smoke checks must target an absolute HTTP(S) loopback URL.'
}
Add-Type -AssemblyName System.Net.Http

$handler = [System.Net.Http.HttpClientHandler]::new()
$handler.AllowAutoRedirect = $false
$handler.CookieContainer = [System.Net.CookieContainer]::new()
$client = [System.Net.Http.HttpClient]::new($handler)
$client.BaseAddress = $BaseUri
$client.Timeout = [TimeSpan]::FromSeconds(30)

function Assert-Response {
    param(
        [string] $Path,
        [int] $Status = 200,
        [string] $Pattern,
        [System.Net.Http.HttpContent] $Content
    )

    $response = if ($null -eq $Content) {
        $client.GetAsync($Path).GetAwaiter().GetResult()
    } else {
        $client.PostAsync($Path, $Content).GetAwaiter().GetResult()
    }

    try {
        $body = $response.Content.ReadAsStringAsync().GetAwaiter().GetResult()
        if ([int] $response.StatusCode -ne $Status) {
            throw "$Path returned $([int] $response.StatusCode); expected $Status."
        }
        if ($Pattern -and $body -notmatch $Pattern) {
            throw "$Path did not contain the expected content."
        }
        if ($Status -eq 302) {
            $location = [string] $response.Headers.Location
            $expected = if ($Path -eq '/Logout') { '/' } elseif ($Path -eq '/Login') { '/Patients' } else { '/Login' }
            $redirectPath = [uri]::new($BaseUri, $location).AbsolutePath
            if ($redirectPath -ne $expected) {
                throw "$Path redirected to $redirectPath; expected $expected."
            }
        }
        Write-Host "PASS $Path ($Status)"
        return $body
    } finally {
        $response.Dispose()
    }
}

function New-LoginContent {
    param([string] $Token, [string] $Password)

    $fields = [System.Collections.Generic.Dictionary[string, string]]::new()
    $fields.Add('Username', $Credential.UserName)
    $fields.Add('Password', $Password)
    if ($Token) {
        $fields.Add('__RequestVerificationToken', $Token)
    }
    return [System.Net.Http.FormUrlEncodedContent]::new($fields)
}

try {
    $null = Assert-Response '/' 200 'CONTOSO'
    $doctors = Assert-Response '/Doctors'
    if ([regex]::Matches($doctors, '/images/doctors/doctor\d+\.jpg').Count -ne 8) {
        throw 'Expected exactly eight seeded doctors. Use a fresh local SQLite database.'
    }
    $null = Assert-Response '/DoctorDetail/1'
    $null = Assert-Response '/DoctorDetail/-1' 404
    $null = Assert-Response '/Patients' 302
    $null = Assert-Response '/PatientDetail/1' 302
    $null = Assert-Response '/css/site.css'
    $null = Assert-Response '/lib/bootstrap/dist/css/bootstrap.min.css'
    $null = Assert-Response '/images/doctors/doctor1.jpg'

    $login = Assert-Response '/Login'
    $tokenMatch = [regex]::Match($login, 'name="__RequestVerificationToken"[^>]*value="([^"]+)"')
    if (-not $tokenMatch.Success) {
        throw 'The login form did not emit an antiforgery token.'
    }
    $token = [System.Net.WebUtility]::HtmlDecode($tokenMatch.Groups[1].Value)
    $password = $Credential.GetNetworkCredential().Password
    $content = New-LoginContent '' $password
    try {
        $null = Assert-Response '/Login' 400 -Content $content
    } finally {
        $content.Dispose()
    }

    $client.DefaultRequestHeaders.Add('Origin', 'https://cross-origin.invalid')
    $client.DefaultRequestHeaders.Add('Sec-Fetch-Site', 'cross-site')
    $content = New-LoginContent '' $password
    try {
        $null = Assert-Response '/Login' 400 -Content $content
    } finally {
        $content.Dispose()
        $null = $client.DefaultRequestHeaders.Remove('Origin')
        $null = $client.DefaultRequestHeaders.Remove('Sec-Fetch-Site')
    }

    $content = New-LoginContent $token ([guid]::NewGuid().ToString())
    try {
        $null = Assert-Response '/Login' 200 'Invalid password' -Content $content
    } finally {
        $content.Dispose()
    }

    $content = New-LoginContent $token $password
    try {
        $null = Assert-Response '/Login' 302 -Content $content
    } finally {
        $content.Dispose()
    }

    $patients = Assert-Response '/Patients'
    if ([regex]::Matches($patients, '/images/patients/patient\d+\.jpg').Count -ne 8) {
        throw 'Expected exactly eight seeded patients. Use a fresh local SQLite database.'
    }
    $null = Assert-Response '/PatientDetail/1'
    $null = Assert-Response '/PatientDetail/-1' 404
    $null = Assert-Response '/Logout' 302
    $null = Assert-Response '/Patients' 302
    Write-Host 'All workload smoke checks passed.'
} finally {
    $client.Dispose()
}
