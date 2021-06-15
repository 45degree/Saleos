#!/bin/bash

if [ ! -d './testCoverageReport' ]; then
rm -rf ./testCoverageReport
fi

dotnet test /p:CollectCoverage=true /p:CoverletOutput='./testCoverageReport/' /p:CoverletOutputFormat=opencover

dotnet reportgenerator "-reports:./testCoverageReport/coverage.opencover.xml" "-targetdir:./testCoverageReport" -reporttypes:Html
