﻿using AppCore.Entities;
using AppCore.Interface.Repositores;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        private const string DATABASE = "testeGps";
        private readonly IMongoClient _mongoClient;
        private readonly IClientSessionHandle _clientSessionHandle;
        private readonly string _collection;

        public BaseRepository(IMongoClient mongoClient, IClientSessionHandle clientSessionHandle, string collection)
        {
            (_mongoClient, _clientSessionHandle, _collection) = (mongoClient, clientSessionHandle, collection);

            if (!_mongoClient.GetDatabase(DATABASE).ListCollectionNames().ToList().Contains(collection))
                _mongoClient.GetDatabase(DATABASE).CreateCollection(collection);
        }

        protected virtual IMongoCollection<T> Collection =>
       _mongoClient.GetDatabase(DATABASE).GetCollection<T>(_collection);

        public async Task InsertAsync(T obj) {
                        
            await Collection.InsertOneAsync(_clientSessionHandle, obj);
        }

        public async Task UpdateAsync(T obj)
        {
            Expression<Func<T, string>> func = f => f.Id;
            var value = (string)obj.GetType().GetProperty(func.Body.ToString().Split(".")[1]).GetValue(obj,null);
            var filter = Builders<T>.Filter.Eq(func, value);


            if (obj != null)
                await Collection.ReplaceOneAsync(_clientSessionHandle, filter, obj);
        }

        public async Task DeleteAsync(string id) =>
            await Collection.DeleteOneAsync(_clientSessionHandle, f => f.Id == id);

        public async Task<IEnumerable<T>> GeAll() => await Collection.Find(_ => true).ToListAsync();

        public async Task<T> Get(string id)
        {
            var filter = Builders<T>.Filter.Eq(s => s.Id, id);
            return await Collection.Find(filter).FirstOrDefaultAsync();

        }


        //public int TimeService { get; set; } = 30000;
        //public int DelayInMilliseconds { get; set; } = 10000;
        //public string MSGErro(string msg, string url, string param)
        //{
        //    if (string.IsNullOrEmpty(msg))
        //    {
        //        msg = $"Erro tratar a chamada da url:> {url} com param => {param}";
        //    }
        //    if (msg.Contains("cance"))
        //        msg += string.Format("Cancelado ", msg, url, param);
        //    else if (!msg.Contains("Erro "))
        //        msg += string.Format("Erro", url, param);

        //    //new LogServico().Save(msg, _Servico);

        //    return msg;
        //}
    }
}
