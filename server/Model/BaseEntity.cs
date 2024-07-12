using System;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Server.Model
{
    /// <summary>
    /// Base class for entities
    /// </summary>
    [Serializable]
    public abstract class BaseEntity
    {
        #region Fields

        public static Random random = new Random();
        
        #endregion

        #region Properties

        public long Id { get; set; }

        public long CreatedDate { get; set; }

        public long ModifiedDate { get; set; }

        public bool IsDeleted { get; set; } = false;

        #endregion

        #region Constructors

        public BaseEntity()
        {
            var now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            CreatedDate = now;
            ModifiedDate = now;
        }

        #endregion

        #region Methods

        public static string GenId()
        {
            string id = string.Empty;
            for (int i = 0; i < 11; i++)
            {
                if (i > 0)
                {
                    id += "-";
                }
                id += random.Next(1000, 9999).ToString();
            }

            return id;
        }

        public static long GenLongId()
        {
            string id = string.Empty;
            for (int i = 0; i < 6; i++)
            {
                id += random.Next(100, 999).ToString();
            }
            return long.Parse(id);
        }

        public static int GenIntId()
        {
            string id = string.Empty;
            for (int i = 0; i < 3; i++)
            {
                id += random.Next(100, 999).ToString();
            }
            return int.Parse(id);
        }

        public string GetAction()
        {
            if (IsDeleted) return "Delete";
            return Id > 0 ? "Update" : "Create";
        }

        #endregion

    }
}
