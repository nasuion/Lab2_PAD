SET SERVER=LabServer\bin\Debug\LabServer.exe
SET CLIENT=PAD\bin\Debug\PAD.exe


echo "Starting server ..."
start %SERVER% 44441 0
start %SERVER% 44442 2 44443 44446
start %SERVER% 44443 3 44442 44445 44446
start %SERVER% 44444 0
start %SERVER% 44445 2 44443 44446
start %SERVER% 44446 3 44443 44444 44445


start %CLIENT%
pause