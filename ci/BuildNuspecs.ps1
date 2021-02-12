Param(
	[Parameter(Mandatory=$true)][string]$path,
	[Parameter(Mandatory=$true)][string]$buildPath,
	[Parameter(Mandatory=$true)][string]$version,
	[Parameter(Mandatory=$false)][string]$branch
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

if (Test-Path $buildPath) {
	Remove-Item $buildPath -Recurse
}

New-Item -Path $buildPath -ItemType Directory

if ($version) {

	$nsUri = 'http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd'
	$ns = @{ns=$nsUri}
	$commit = (git rev-parse HEAD)
	if (-not $branch) {
		$branch = (git rev-parse --abbrev-ref HEAD)
	}

	Get-ChildItem $path | ForEach {
		$xmlPath = Resolve-Path $_.FullName

		$xml = [xml] (Get-Content "$xmlPath")
		$xml.PreserveWhitespace = $true

		Select-Xml -Xml $xml -XPath '//ns:metadata/ns:version' -Namespace $ns |
		Select -expand node |
		ForEach { $_.InnerText = $version }

		$child = $xml.CreateElement('version', $nsUri)
		$child.InnerText = $version
		$xml.package.metadata.AppendChild($child)

		$child = $xml.CreateElement('releaseNotes', $nsUri)
		$child.InnerText = 'https://github.com/MaceWindu/sourcemap-toolkit/releases/tag/v' + $version
		$xml.package.metadata.AppendChild($child)

		$child = $xml.CreateElement('repository', $nsUri)
		$attr = $xml.CreateAttribute('type')
		$attr.Value = 'git'
		$child.Attributes.Append($attr)
		$attr = $xml.CreateAttribute('url')
		$attr.Value = 'https://github.com/linq2db/linq2db.git'
		$child.Attributes.Append($attr)
		$attr = $xml.CreateAttribute('branch')
		$attr.Value = $branch
		$child.Attributes.Append($attr)
		$attr = $xml.CreateAttribute('commit')
		$attr.Value = $commit
		$child.Attributes.Append($attr)
		$xml.package.metadata.AppendChild($child)

		Write-Host "Patched $xmlPath"
		$xml.Save($buildPath + '\' + [System.IO.Path]::GetFileName($xmlPath))
	}
}
