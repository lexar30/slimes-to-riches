using System;
using System.Collections.Generic;
using SlimesToRiches.Arena.Entities.Slimes;
using UnityEngine;

namespace SlimesToRiches.Arena.Spatial
{
    public sealed class SlimeUniformGrid
    {
        private struct Membership
        {
            public int CellIndex;
            public int IndexInsideCell;
        }

        private readonly int columns;
        private readonly int rows;
        private readonly List<SlimeRuntimeState>[] cells;
        private readonly Dictionary<SlimeRuntimeState, Membership> memberships = new();

        public int Columns => columns;
        public int Rows => rows;
        public int Count => memberships.Count;

        public SlimeUniformGrid(int columns, int rows)
        {
            if (columns <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(columns));
            }

            if (rows <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rows));
            }

            this.columns = columns;
            this.rows = rows;

            cells = new List<SlimeRuntimeState>[columns * rows];
            for (int i = 0; i < cells.Length; ++i)
            {
                cells[i] = new List<SlimeRuntimeState>();
            }
        }

        public bool Add(SlimeRuntimeState slime)
        {
            if (slime == null || memberships.ContainsKey(slime))
            {
                return false;
            }

            int cellIndex = GetCellIndex(slime.NormalizedPosition);
            List<SlimeRuntimeState> cell = cells[cellIndex];

            memberships.Add(slime, new Membership
            {
                CellIndex = cellIndex,
                IndexInsideCell = cell.Count
            });

            cell.Add(slime);
            return true;
        }

        public bool Remove(SlimeRuntimeState slime)
        {
            if (slime == null || !memberships.TryGetValue(slime, out Membership membership))
            {
                return false;
            }

            RemoveFromCell(membership);
            memberships.Remove(slime);
            return true;
        }

        public bool UpdateCell(SlimeRuntimeState slime)
        {
            if (slime == null || !memberships.TryGetValue(slime, out Membership membership))
            {
                return false;
            }

            int newCellIndex = GetCellIndex(slime.NormalizedPosition);
            if (newCellIndex == membership.CellIndex)
            {
                return false;
            }

            RemoveFromCell(membership);

            List<SlimeRuntimeState> newCell = cells[newCellIndex];
            memberships[slime] = new Membership
            {
                CellIndex = newCellIndex,
                IndexInsideCell = newCell.Count
            };

            newCell.Add(slime);
            return true;
        }

        public IReadOnlyList<SlimeRuntimeState> GetCell(int column, int row)
        {
            if ((uint)column >= (uint)columns)
            {
                throw new ArgumentOutOfRangeException(nameof(column));
            }

            if ((uint)row >= (uint)rows)
            {
                throw new ArgumentOutOfRangeException(nameof(row));
            }

            return cells[row * columns + column];
        }

        public void GetCandidates(
            Vector2 normalizedMin,
            Vector2 normalizedMax,
            List<SlimeRuntimeState> result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            result.Clear();

            float minX = Mathf.Clamp01(Mathf.Min(normalizedMin.x, normalizedMax.x));
            float minY = Mathf.Clamp01(Mathf.Min(normalizedMin.y, normalizedMax.y));
            float maxX = Mathf.Clamp01(Mathf.Max(normalizedMin.x, normalizedMax.x));
            float maxY = Mathf.Clamp01(Mathf.Max(normalizedMin.y, normalizedMax.y));

            int minColumn = GetColumn(minX);
            int minRow = GetRow(minY);
            int maxColumn = GetColumn(maxX);
            int maxRow = GetRow(maxY);

            for (int row = minRow; row <= maxRow; ++row)
            {
                int rowOffset = row * columns;
                for (int column = minColumn; column <= maxColumn; ++column)
                {
                    result.AddRange(cells[rowOffset + column]);
                }
            }
        }

        public void Clear()
        {
            for (int i = 0; i < cells.Length; ++i)
            {
                cells[i].Clear();
            }

            memberships.Clear();
        }

        private int GetCellIndex(Vector2 normalizedPosition)
        {
            return GetRow(normalizedPosition.y) * columns + GetColumn(normalizedPosition.x);
        }

        private int GetColumn(float normalizedX)
        {
            return Mathf.Clamp(Mathf.FloorToInt(normalizedX * columns), 0, columns - 1);
        }

        private int GetRow(float normalizedY)
        {
            return Mathf.Clamp(Mathf.FloorToInt(normalizedY * rows), 0, rows - 1);
        }

        private void RemoveFromCell(Membership membership)
        {
            List<SlimeRuntimeState> cell = cells[membership.CellIndex];
            int lastIndex = cell.Count - 1;
            SlimeRuntimeState movedSlime = cell[lastIndex];

            cell[membership.IndexInsideCell] = movedSlime;
            cell.RemoveAt(lastIndex);

            if (membership.IndexInsideCell != lastIndex)
            {
                Membership movedMembership = memberships[movedSlime];
                movedMembership.IndexInsideCell = membership.IndexInsideCell;
                memberships[movedSlime] = movedMembership;
            }
        }
    }
}
