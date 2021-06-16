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

using Saleos.DAO;

namespace Saleos.Entity.DAOExtension
{
    public static class CategoryDAOExtension
    {
        public static CategoryDAO GetCategoryDAOFromCategory(this Category category)
        {
            if (category == null) return null;
            return new CategoryDAO()
            {
                Id = category.Id,
                Content = category.Content
            };
        }

        public static Category GetCategoryFromCategoryDAO(this CategoryDAO categoryDAO)
        {
            if (categoryDAO == null) return null;
            return new Category
            {
                Id = categoryDAO.Id,
                Content = categoryDAO.Content,
            };
        }

        public static Category GetCategoryFromCateGoryAddDAO(this CategoryAddDAO categoryAddDAO)
        {
            if (categoryAddDAO == null) return null;
            return new Category()
            {
                Content = categoryAddDAO.Content,
            };
        }
    }
}
