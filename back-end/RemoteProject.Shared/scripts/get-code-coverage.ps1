#!/usr/bin/env powershell

[XML]$report = Get-Content $args[0]

Write-Host $report.coverage.'line-rate'
