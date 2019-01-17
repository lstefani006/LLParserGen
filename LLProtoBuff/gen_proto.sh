#!/bin/bash

set -e
set -x

DN="/c/Programmi/dotnet/dotnet.exe"

"$DN" ./bin/Debug/netcoreapp2.1/LLProtoBuff.dll -cs  PbMsg.txt > ../../et_tariff_engine_service/et_tariff_engine/q.cs
"$DN" ./bin/Debug/netcoreapp2.1/LLProtoBuff.dll -cpp PbMsg.txt > ../../et_tariff_engine_service/PbT2lib/PbMsg.cpp
"$DN" ./bin/Debug/netcoreapp2.1/LLProtoBuff.dll -hpp PbMsg.txt > ../../et_tariff_engine_service/PbT2lib/PbMsg.hpp
