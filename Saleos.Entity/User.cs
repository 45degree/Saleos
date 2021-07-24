/*
 * copyright 2021 45degree
 *
 * licensed under the apache license, version 2.0 (the "license");
 * you may not use this file except in compliance with the license.
 * you may obtain a copy of the license at
 *
 * http://www.apache.org/licenses/license-2.0
 *
 * unless required by applicable law or agreed to in writing, software
 * distributed under the license is distributed on an "as is" basis,
 * without warranties or conditions of any kind, either express or implied.
 * see the license for the specific language governing permissions and
 * limitations under the license.
 */

using System;
using System.Collections.Generic;

namespace Saleos.Entity
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string PasswordSha1 { get; set; }

        public List<Role> Roles { get; set; }

        public string Salt { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime LastLoginTime { get; set; }

        public DateTime LastLogoutTime { get; set; }

        public bool IsLogin { get; set; }
    }
}
