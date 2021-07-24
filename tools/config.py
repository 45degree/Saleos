#!/usr/bin/python
#
# Copyright 2021 45degree
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
# http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.
#

from io import TextIOWrapper
from typing import MutableMapping
import toml
import sys
import argparse

class Config:
    '''
    this class use to generate config file for the project
    '''
    def __init__(self, configFilePath: str) -> None:
        self.__isParsed: bool = False
        self.__filePath: str = configFilePath
        self.__postgres_host: str = ""
        self.__postgres_port: int = 5432
        self.__postgres_user: str = "postgres"
        self.__postgres_password: str = "postgres"
        self.__minio_endpoint: str = "localhost:9000"
        self.__minio_consolePoint: str = "localhost:9001"
        self.__minio_accesskey: str = "saleosadmin"
        self.__minio_security: str = "saleosadmin"
        self.__minio_bucketname: str = "article"
        self.__SaleosAdmin_port: int = 3232
        self.__Admin_Name: str = "Saleos"
        self.__Admin_Password: str = "Saleos"

    def parse(self) -> None:
        '''
        parse the specified toml file
        '''
        tomlFile = toml.load(self.__filePath)
        self.__parsePostGres(tomlFile)
        self.__parseMinio(tomlFile)
        self.__parseSaleosAdmin(tomlFile)
        self.__isParsed = True

    def generateEnvFile(self, outPath: str) -> None:
        '''
        save the environment variable into a file.
        format: [variable]=[value]
        '''
        if self.__isParsed == False :
            self.parse()
        envFile: TextIOWrapper = open(outPath, "w+")
        envFile.writelines(self.__getEnvVariable())

    def print(self) -> None:
        '''
        print environment variable to the console.
        format: [variable]=[value]
        '''
        sys.stdout.writelines(self.__getEnvVariable())


    def __getEnvVariable(self) -> list[str]:
        if self.__isParsed == False :
            self.parse()

        envVariable: list[str] = []

        # docker config

        ## if you don't use this environment variable, docker will build failed in Linux
        ## because the forbidden path outside the build context.
        ## see: https://github.com/docker/cli/issues/3102
        ##      https://github.com/moby/buildkit/issues/2130
        ##      https://github.com/moby/buildkit/issues/2131
        envVariable.append("DOCKER_BUILDKIT=1\n")

        # postgres environment variable
        envVariable.append("{}={}\n".format("POSTGRES_USER", self.__postgres_user))
        envVariable.append("{}={}\n".format("POSTGRES_PASSWORD", self.__postgres_password))
        envVariable.append("{}={}\n".format("POSTGRES_HOST", self.__postgres_host))
        envVariable.append("{}={}\n".format("POSTGRES_PORT", self.__postgres_port))

        # minio environment variable
        envVariable.append("{}={}\n".format("MINIO_ENDPOINT", self.__minio_endpoint))
        envVariable.append("{}={}\n".format("MINIO_CONSOLE_PORT", self.__minio_consolePoint))
        envVariable.append("{}={}\n".format("MINIO_ACCESSKEY", self.__minio_accesskey))
        envVariable.append("{}={}\n".format("MINIO_SECURITY", self.__minio_security))
        envVariable.append("{}={}\n".format("MINIO_BUCKETNAME", self.__minio_bucketname))

        # saleos.Admin environment variable
        envVariable.append("{}={}\n".format("SALEOS_ADMIN_PORT", self.__SaleosAdmin_port))

        # admin environment variable
        envVariable.append("{}={}\n".format("SALEOS_ADMIN_NAME", self.__Admin_Name))
        envVariable.append("{}={}\n".format("SALEOS_ADMIN_PASSWORD", self.__Admin_Password))

        return envVariable

    def __parsePostGres(self, tomlFile: MutableMapping[str, any]) -> None:
        postgresConfig = tomlFile["Postgres"]
        self.__postgres_host = postgresConfig["host"]
        self.__postgres_user = postgresConfig["user"]
        self.__postgres_password = postgresConfig["password"]
        self.__postgres_port = postgresConfig["port"]


    def __parseMinio(self, tomlFile: MutableMapping[str, any]) -> None:
        minioConfig: MutableMapping[str, any] = tomlFile["minio"]
        self.__minio_endpoint = "{}:{}".format(minioConfig["host"], minioConfig["endpoint_port"])
        self.__minio_consolePoint = minioConfig["console_port"]
        self.__minio_accesskey = minioConfig["accesskey"]
        self.__minio_security = minioConfig["security"]
        self.__minio_bucketname = minioConfig["bucketname"]

    def __parseSaleosAdmin(self, tomlFile: MutableMapping[str, any]):
        saleosAdminConfig: MutableMapping[str, any] = tomlFile["Saleos"]["Admin"]
        self.__SaleosAdmin_port = saleosAdminConfig["port"]

    def __parseAdmin(self, tomlFile: MutableMapping[str, any]):
        self.__Admin_Name = tomlFile["Admin"]["Name"]
        self.__Admin_Password = tomlFile["Admin"]["Password"]

if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument("configFile", help="the path of config.toml")
    parser.add_argument("-o", help="the output path of .env file")
    args = parser.parse_args()
    config = Config(args.configFile)
    config.parse()
    if args.o:
        config.generateEnvFile(args.o)
    else:
        config.print()
