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
using Saleos.Test.Entity;

namespace Saleos.Test
{
    public class MemoryEntityTest : EntityTest
    {
        public MemoryEntityTest() : base(
            new DbContextOptionsBuilder<HomePageDbContext>()
                .UseInMemoryDatabase("Mock CoreDatabase").Options,
            new DbContextOptionsBuilder<IdentityDbContext>()
                .UseInMemoryDatabase("Mock IdentityDatabase").Options)
        {
        }
    }

    public class MemoryArticleRepositoryTest : ArticleRepositoryTest
    {
        public MemoryArticleRepositoryTest() : base(
            new DbContextOptionsBuilder<HomePageDbContext>()
                .UseInMemoryDatabase("Mock ArticleRepository-routine")
                .Options)
        {
        }
    }

    public class MemoryArticleInfoRepositoryTest : ArticleInfoRepositoryTest
    {
        public MemoryArticleInfoRepositoryTest(): base(
            new DbContextOptionsBuilder<HomePageDbContext>()
                .UseInMemoryDatabase("Mock ArticleInfoRepository-routine")
                .Options)
        {

        }
    }

    public class MemoryTagInfoRepositoryTest : TagRepositoryTest
    {
        public MemoryTagInfoRepositoryTest(): base(
            new DbContextOptionsBuilder<HomePageDbContext>()
                .UseInMemoryDatabase("Mock TagRepository-routine")
                .Options)
        {
        }
    }

    public class MemoryCategoryRepositoryTest : CategoryRepositoryTest
    {
        public MemoryCategoryRepositoryTest()
            : base(new DbContextOptionsBuilder<HomePageDbContext>()
                .UseInMemoryDatabase("Mock CategoryRepository-routine")
                .Options)
        {
        }
    }
}
