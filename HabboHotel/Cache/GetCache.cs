using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aleeda.HabboHotel.Cache
{
    public class GetCache
    {
        #region Fields
        private cataloguePages cataPages;
        private catalogueItems cataItems;
        private PrivateRooms privRooms;
        private WallItems wallItems;
        private FloorItems floorItems;
        private RoomBots roomBots;
        private RoomFavourites roomFav;
        private CatalogueIndex cataIndex;
        #endregion

        #region Constructor
        public GetCache()
        {
            cataPages = new cataloguePages();
            cataItems = new catalogueItems();
            privRooms = new PrivateRooms();
            wallItems = new WallItems();
            floorItems = new FloorItems();
            roomBots = new RoomBots();
            roomFav = new RoomFavourites();
            cataIndex = new CatalogueIndex();
        }
        #endregion

        #region Methods
        public cataloguePages GetCataloguePage()
        {
            return cataPages;
        }
        public catalogueItems GetCatalogueItems()
        {
            return cataItems;
        }
        public PrivateRooms GetPrivateRooms()
        {
            return privRooms;
        }
        public WallItems GetWallItems()
        {
            return wallItems;
        }
        public FloorItems GetFloorItems()
        {
            return floorItems;
        }
        public RoomBots GetRoomBots()
        {
            return roomBots;
        }
        public RoomFavourites GetRoomFavs()
        {
            return roomFav;
        }
        public CatalogueIndex GetCataIndex()
        {
            return cataIndex;
        }
        #endregion
    }
}
