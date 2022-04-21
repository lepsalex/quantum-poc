using System.Collections.Generic;

namespace Quantum.CustomState.Helpers
{
    public class PlayerColorHelper
    {
        public static PlayerColor GetPlayerColor(PlayerRef playerRef)
        {
            return _playerColors[(playerRef._index - 1) % 6];
        }
        
        private static Dictionary<int, PlayerColor> _playerColors = new()
        {
            { 0, new PlayerColor{
                R = 130,
                G = 2,
                B = 99,
                A = 255
            }},
            { 1, new PlayerColor{
                R = 217,
                G = 3,
                B = 104,
                A = 255
            }},
            { 2, new PlayerColor{
                R = 234,
                G = 222,
                B = 218,
                A = 255
            }},
            { 3, new PlayerColor{
                R = 46,
                G = 41,
                B = 78,
                A = 255
            }},
            { 4, new PlayerColor{
                R = 255,
                G = 212,
                B = 0,
                A = 255
            }},
            { 5, new PlayerColor{
                R = 173,
                G = 252,
                B = 249,
                A = 255
            }},
        };
    }
}