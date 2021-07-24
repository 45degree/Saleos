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
using Saleos.Test.Entity.CoreServicesTest;

namespace Saleos.Test
{
    public class SqliteEntityTest : EntityTest
    {
        public SqliteEntityTest() : base(new DbContextOptionsBuilder<HomePageDbContext>()
            .UseSqlite("Data Source=Mock-Database.db")
            .Options)
        {
        }
    }

    public class SqliteArticleRepositoryTest : ArticleRepositoryTest
    {
        public SqliteArticleRepositoryTest() : base(new DbContextOptionsBuilder<HomePageDbContext>()
            .UseSqlite("Data Source=Mock-ArticleRepository-routine.db")
            .Options)
        {
        }
    }

    public class SqliteArticleInfoRepositoryTest : ArticleInfoRepositoryTest
    {
        public SqliteArticleInfoRepositoryTest(): base(new DbContextOptionsBuilder<HomePageDbContext>()
            .UseSqlite("Data Source=Mock-ArticleInfoRepository-routine.db")
            .Options)
        {

        }
    }

    public class SqliteTagInfoRepositoryTest : TagRepositoryTest
    {
        public SqliteTagInfoRepositoryTest(): base(new DbContextOptionsBuilder<HomePageDbContext>()
            .UseSqlite("Data Source=Mock-TagRepository-routine.db")
            .Options)
        {
        }
    }

    public class SqliteCategoryRepositoryTest : CategoryRepositoryTest
    {
        public SqliteCategoryRepositoryTest()
            : base(new DbContextOptionsBuilder<HomePageDbContext>()
                .UseSqlite("Data Source=Mock-CategoryRepository-routine.db")
                .Options)
        {
        }
    }
}
