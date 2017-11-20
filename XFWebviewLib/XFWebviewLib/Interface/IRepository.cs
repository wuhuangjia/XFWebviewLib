﻿using System;
using System.Collections.Generic;
using System.Text;

namespace XFWebviewLib.Interface
{
    public interface IRepository<TEntity> : IDisposable
        where TEntity : class
    {
        void Insert(TEntity entity);
        void InsertAll(IList<TEntity> entities);
        void Update(TEntity entity);
        void UpdateAll(IList<TEntity> entities);
        void Delete(TEntity entity);
        void DeleteAll(IList<TEntity> entities);
        IList<TEntity> GetAll();

    }
}
