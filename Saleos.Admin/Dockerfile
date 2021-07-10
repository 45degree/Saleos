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

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ../Saleos.Admin ./Saleos.Admin
COPY ../Saleos.DAO ./Saleos.DAO
COPY ../Saleos.Entity ./Saleos.Entity
WORKDIR "/src/Saleos.Admin"
RUN dotnet build "Saleos.Admin.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Saleos.Admin.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
ENV POSTGRES_USER=Saleos \
    POSTGRES_PASSWORD=Saleos \
    POSTGRES_PORT=5432 \
    POSTGRES_HOST=localhost \
    MINIO_ENDPOINT=localhost:9000 \
    MINIO_ACCESSKEY=minioadmin \
    MINIO_SECURITY=minioadmin
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Saleos.Admin.dll"]
