using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy.MongoDTO;
using MongoDB.Driver.Builders;
using MongoDB.Driver;

namespace Jinher.AMP.BTP.BE.MongoCollection
{
    public class MongoCollections
    {
        static MongoCollections()
        {
            var db = MongoManager.getDB();
            var store = db.GetCollection<StoreMgDTO>(MongoKeyConst.CollectionName);
            var geoindexMapLocation = IndexKeys<StoreMgDTO>.GeoSpatial(c => c.Location);
            store.EnsureIndex(geoindexMapLocation, IndexOptions.SetGeoSpatialRange(0, 300));
        }
        public static MongoCollection<StoreMgDTO> Store
        {
            get
            {
                return MongoManager.getDB().GetCollection<StoreMgDTO>(MongoKeyConst.CollectionName);
            }
        }
    }
}
