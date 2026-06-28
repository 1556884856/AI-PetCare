$listener = [System.Net.Sockets.TcpListener]::new([System.Net.IPAddress]::Loopback, 18888)
$listener.Start()
Write-Host "proxy ready on 18888"

$task = [System.Threading.Tasks.Task]::Run({
    while ($true) {
        $client = $listener.AcceptTcpClient()
        [System.Threading.Tasks.Task]::Run({
            try {
                $ns = $client.GetStream()
                $reader = New-Object System.IO.StreamReader($ns)
                $line = $reader.ReadLine()
                if ($line -match "^CONNECT (.+):(\d+)") {
                    $host = $Matches[1]
                    $port = [int]$Matches[2]
                    $github = if ($host -eq "github.com") { "20.205.243.166" } else { $host }
                    $remote = New-Object System.Net.Sockets.TcpClient($github, $port)
                    $rs = $remote.GetStream()
                    $writer = New-Object System.IO.StreamWriter($ns)
                    $writer.Write("HTTP/1.1 200 Connection Established`r`n`r`n")
                    $writer.Flush()
                    $t1 = [System.Threading.Tasks.Task]::Run({ try { $ns.CopyTo($rs) } catch {} })
                    $t2 = [System.Threading.Tasks.Task]::Run({ try { $rs.CopyTo($ns) } catch {} })
                    [System.Threading.Tasks.Task]::WaitAny(@($t1, $t2))
                }
            } catch {} finally { $client.Dispose() }
        })
    }
})
