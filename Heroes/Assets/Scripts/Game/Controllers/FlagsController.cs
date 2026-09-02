using System.Collections.Generic;
using System.Linq;
using Enums;
using Game.Battle;
using Game.Flags;
using UnityEngine;

namespace Game.Controllers
{
    public class FlagsController : MonoBehaviour
    {
        [SerializeField] private List<FlagUnit> _flags;
        [SerializeField] private int _flagCaptureDistance = 2;
        private Dictionary<PlayerType, int> _flagsCount = new Dictionary<PlayerType, int>
        {
            {PlayerType.None, 0},
            {PlayerType.Player, 0},
            {PlayerType.Enemy, 0}
        };

        private GameMachine GameMachine => GameController.Instance.GameMachine;

        private void Awake()
        {
            if (GameController.Instance.FlagsController != null)
            {
                Debug.LogError("Flags already initiated");
                return;
            }
            
            GameController.Instance.FlagsController = this;
        }

        private void FixedUpdate()
        {
            CheckDistances();
        }

        private void CheckDistances()
        {
            _flagsCount = new Dictionary<PlayerType, int>
            {
                {PlayerType.None, 0},
                {PlayerType.Player, 0},
                {PlayerType.Enemy, 0}
            };
            
            var flagsActive = new Dictionary<FlagUnit, Dictionary<PlayerType, int>>();
            
            if (GameMachine.Cells.Length == 0)
                return;

            int midCell;
            int captureDistance;
            if (GameMachine.HorizontalCellCount % 2 == 0)
            {
                midCell = GameMachine.HorizontalCellCount / 2 - 1;
                captureDistance = _flagCaptureDistance * 2;
            } else
            {
                midCell = GameMachine.HorizontalCellCount / 2 + GameMachine.HorizontalCellCount % 2 - 1;
                captureDistance = _flagCaptureDistance * 2 + 1;
            }
            
            var cells = Enumerable.Range(midCell - _flagCaptureDistance, captureDistance).ToList();
            
            foreach (var flag in _flags)
            {
                flagsActive[flag] = new Dictionary<PlayerType, int>
                {
                    {PlayerType.None, 0},
                    {PlayerType.Player, 0},
                    {PlayerType.Enemy, 0}
                };

                foreach (var posX in cells)
                {
                    var cell = GameMachine.Cells[posX, (int) flag.Line];
                    
                    if (cell is null || cell.CellUnitCollection.Count == 0)
                        continue;
                    
                    foreach (var unit in cell.CellUnitCollection)
                    {
                        if (unit is null || unit.Health <= 0) continue;
                        
                        flagsActive[flag][unit.PlayerType]++;
                    }
                }

                if (flagsActive[flag][PlayerType.Player] > flagsActive[flag][PlayerType.Enemy])
                    flag.Capture(PlayerType.Player);
                else if (flagsActive[flag][PlayerType.Player] < flagsActive[flag][PlayerType.Enemy])
                    flag.Capture(PlayerType.Enemy);
                
                _flagsCount[flag.Player]++;
            }
        }

        public int GetFlagsCount(PlayerType player)
        {
            return _flagsCount[player];
        }

        public PlayerType GetFlagOwnerOnLine(LineType line)
        {
            return _flags.FirstOrDefault(x => x.Line == line).Player;
        }
    }
}