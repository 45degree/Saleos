/*
 * Copyright 2021 45degree
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Microsoft.EntityFrameworkCore;
using Saleos.Entity.Data;

namespace Saleos.Test.Entity.CoreServicesTest
{
    public class BaseCoreServicesTest
    {
        protected DbContextOptions<HomePageDbContext> ContextOptions { get; }
        protected MockData _mockData = MockData.GetInstance();

        protected BaseCoreServicesTest(DbContextOptions<HomePageDbContext> contextOptions)
        {
            ContextOptions = contextOptions;
            Seed();
        }

        private void Seed()
        {
            using var context = new HomePageDbContext(ContextOptions);
            MockData.SeedData(context);
        }
    }
}
