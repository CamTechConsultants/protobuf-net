cls
del protobuf-net-mcs.dll
del mcs-test.exe

mcs -recurse:protobuf-net\*.cs -out:protobuf-net-mcs.dll -target:library -unsafe+ -define:FX11 -define:FEAT_COMPILER

mcs -recurse:SampleDto\*.cs -out:dto-mcs.dll -target:library -define:FX11 -define:FEAT_COMPILER -r:protobuf-net-mcs

mcs -recurse:FX11\*.cs -target:exe -out:mcs-test.exe -define:FX11 -define:FEAT_COMPILER -r:protobuf-net-mcs -r:dto-mcs

mono mcs-test.exe