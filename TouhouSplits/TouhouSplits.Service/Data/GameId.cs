using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TouhouSplits.Service.Data
{
    [DataContract]
    public struct GameId
    {
        [DataMember]
        public string Id { get; private set; }

        public GameId(string id)
        {
            if (id == null) {
                Id = string.Empty;
            }
            Id = id;
        }

        public override string ToString()
        {
            return Id.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is GameId)) {
                return false;
            }
            return Id.Equals( ((GameId) obj).Id );
        }

        public bool Equals(GameId otherGameId)
        {
            if (otherGameId == null) {
                return false;
            }
            return Id.Equals(otherGameId.Id);
        }

        public override int GetHashCode()
        {
            return 2108858624 + EqualityComparer<string>.Default.GetHashCode(Id);
        }

        public static bool operator ==(GameId a, GameId b)
        {
            return a.Id.Equals(b.Id);
        }

        public static bool operator !=(GameId a, GameId b)
        {
            return !(a == b);
        }
    }
}
