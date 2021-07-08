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

using System;
using System.Collections.Generic;
using Saleos.Entity;
using Saleos.Entity.Data;

namespace Saleos.Test
{
    public class MockData
    {
        public List<Article> Articles {get; set;}
        public List<Category> Categories {get; set;}
        public List<Tag> Tags { get; set;}
        public List<ArticleTag> ArticleTags { get; set;}

        private MockData()
        {
            Categories = new List<Category>(){
                new Category() {
                    Id = 1,
                    Content = "Category 1",
                },
                new Category() {
                    Id = 2,
                    Content = "Category 2"
                }
            };
            Tags = new List<Tag>() {
                new Tag() {
                    Id = 1,
                    Content = "Tag 1",
                },
                new Tag() {
                    Id = 2,
                    Content = "Tag 2",
                },
                new Tag() {
                    Id = 3,
                    Content = "Tag 3",
                }
            };
            Articles = new List<Article>(){
                new Article() {
                    Id = 1,
                    Title =  "Title 1",
                    Abstract = "Abstract 1",
                    Content = "Content 1",
                    ImageUrl = "Url 1",
                    IsReprint = false,
                    Category = Categories[0],
                    CreateTime = new DateTime(2020, 2, 1),
                    LastModifiedTime = DateTime.Today
                },
                new Article() {
                    Id = 2,
                    Title =  "Title 2",
                    Abstract = "Abstract 2",
                    Content = "Content 2",
                    IsReprint = true,
                    ReprintUri = "http://12345.com",
                    ImageUrl = "Url 2",
                    Category = Categories[0],
                    CreateTime = new DateTime(2020, 2, 2),
                    LastModifiedTime = DateTime.Today
                },
                new Article() {
                    Id = 3,
                    Title =  "Title 3",
                    Abstract = "Abstract 3",
                    Content = "Content 3",
                    IsReprint = true,
                    ReprintUri = "http://12345.com",
                    ImageUrl = "Url 2",
                    Category = Categories[1],
                    CreateTime = new DateTime(2020, 2, 3),
                    LastModifiedTime = DateTime.Today
                }
            };
            ArticleTags = new List<ArticleTag>() {
                new ArticleTag {Article = Articles[0], Tag = Tags[0]},
                new ArticleTag {Article = Articles[0], Tag = Tags[1]},
                new ArticleTag {Article = Articles[0], Tag = Tags[2]},
                new ArticleTag {Article = Articles[1], Tag = Tags[1]},
                new ArticleTag {Article = Articles[1], Tag = Tags[2]},
                new ArticleTag {Article = Articles[2], Tag = Tags[2]},
            };
        }

        // private static MockData _instance = new MockData();

        public static MockData GetInstance()
        {
            return new MockData();
        }

        /// <summary>
        /// generator test data in the database
        /// </summary>
        public static void SeedData(HomePageDbContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var mockData = MockData.GetInstance();

            context.Article.AddRange(mockData.Articles);
            context.Tags.AddRange(mockData.Tags);
            context.Categories.AddRange(mockData.Categories);
            context.ArticleTags.AddRange(mockData.ArticleTags);
            context.SaveChanges();
        }
    }
}
