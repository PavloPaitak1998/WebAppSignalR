using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;

using Server_WEB_Programming.Lab4.Models;

namespace Server_WEB_Programming.Lab4.Hubs
{
    public class MyHub1 : Hub
    {
        private static readonly List<User> _users = new List<User>();

        private static readonly Dictionary<Guid, ChatRoom> _roomInfo = new Dictionary<Guid, ChatRoom>();

        public void Send(string name, string message)
        {
            Clients.All.addMessage(name, message);
        }

        public void CreateGroup(string toUserConnectionId, string groupName)
        {
            var roomId = Guid.NewGuid();

            _roomInfo.Add(roomId, new ChatRoom
            {
                GroupConnectionsId = new List<string>
                {
                     toUserConnectionId, Context.ConnectionId
                },
                Name = groupName
            });

            Groups.Add(Context.ConnectionId, roomId.ToString());
            Groups.Add(toUserConnectionId, roomId.ToString());

            Clients.Group(roomId.ToString()).onGroupCreated(groupName);
        }

        public void SendGroupMessage(string name, string message, string groupName)
        {
            var room = _roomInfo.FirstOrDefault(x => x.Value.Name == groupName);

            Clients.Group(room.Key.ToString()).addGroupMessage(name, message, room.Value.Name);
        }

        public void Connect(string userName)
        {
            var id = Context.ConnectionId;


            if (_users.All(x => x.ConnectionId != id))
            {
                _users.Add(new User { ConnectionId = id, Name = userName });

                Clients.Caller.onConnected(id, userName, _users);

                Clients.AllExcept(id).onNewUserConnected(id, userName);
            }
        }

        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            var item = _users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (item != null)
            {
                _users.Remove(item);
                var id = Context.ConnectionId;
                Clients.All.onUserDisconnected(id, item.Name);
            }

            return base.OnDisconnected(stopCalled);
        }
    }
}