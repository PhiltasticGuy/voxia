#!/bin/dash
set -e

echo "##################################################"
echo "SETUP IceGrid"
echo "##################################################"
icegridadmin -u w -p w -e "application remove Simple" \
	|| icegridadmin -u w -p w -e "application add application.xml" \
	|| icegridadmin -u w -p w -e "server start SimpleServer"
echo "##################################################"

echo "##################################################"
echo "STARTING IceGrid Node"
echo "##################################################"
exec "$@"
echo "##################################################"