# netsh http add urlacl url=http://[ip]:[port]/ user=[machine\user] listen=yes delegate=yes
netsh http add urlacl url=http://192.168.1.12:8081/ user=WILT\EZ listen=yes delegate=yes