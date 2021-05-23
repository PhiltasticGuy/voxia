Write-Output "##[START]###################################################"
$application = "Streaming"
$server1 = "MediaServer1"
$server2 = "MediaServer2"

Write-Output " > Removing existing instance of '${application}' IceGrid application."
icegridadmin -u w -p w -e "application remove ${application}" --Ice.Config="src/ice/VoxIA.ZerocIce.Server/config.admin"

Write-Output " > Adding new instance of '${application}' IceGrid application."
icegridadmin -u w -p w -e "application add src/ice/VoxIA.ZerocIce.Server/application.xml" --Ice.Config="src/ice/VoxIA.ZerocIce.Server/config.admin"

Write-Output " > Starting IceGrid server instance '${server1}'."
icegridadmin -u w -p w -e "server start ${server1}" --Ice.Config="src/ice/VoxIA.ZerocIce.Server/config.admin"

Write-Output " > Starting IceGrid server instance '${server2}'."
icegridadmin -u w -p w -e "server start ${server2}" --Ice.Config="src/ice/VoxIA.ZerocIce.Server/config.admin"
Write-Output "##[END]#####################################################"