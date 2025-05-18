using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TactileListExtension;

namespace Tactile
{
    class Turnwheel
    {
        List<Turnwheel_Snapshot> Snapshots = new List<Turnwheel_Snapshot> { };
        public List<Turnwheel_Snapshot> snapshots { get { return Snapshots; } }
        public Turnwheel_Snapshot current_snapshot { get { return Snapshots.Last(); } }
        #region: Serialization
        public void write(BinaryWriter writer)
        {
            Snapshots.write(writer);
        }
        public void read(BinaryReader reader)
        {
            Snapshots.read(reader);
        }
        #endregion

        public Turnwheel() { }
        public void Take_Snapshot()
        {
            string temp_filename = System.IO.Path.GetTempFileName();
            Create_Snapshot(temp_filename);
            Save_Snapshot(temp_filename);
            Delete_Temp_File(temp_filename);
        }
        private void Create_Snapshot(string temp_filename)
        {
            Turnwheel_Snapshot snapshot = new Turnwheel_Snapshot();
            snapshot.Get_Global_Variables();
            using (var stream = File.Open(temp_filename, FileMode.Create))
            {
                using (var writer = new BinaryWriter(stream, Encoding.UTF8))
                    snapshot.write(writer);
            }
        }
        private void Save_Snapshot(string temp_filename)
        {
            using (var stream = File.Open(temp_filename, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream, Encoding.UTF8))
                {
                    Turnwheel_Snapshot snapshot = new Turnwheel_Snapshot();
                    snapshot.read(reader);
                    Snapshots.Add(snapshot);
                }
            }
        }
        private void Delete_Temp_File(string temp_filename)
        {
            if (File.Exists(temp_filename))
            {
                File.Delete(temp_filename);
            }
        }
        public Turnwheel_Snapshot rewind(int index)
        {
            Turnwheel_Snapshot rewound_snapshot = Snapshots[index];
            int number_of_snapshots_to_remove = Snapshots.Count - (index + 1);
            Snapshots.RemoveRange(index + 1, number_of_snapshots_to_remove); // Discard snapshots that take place after the point we're rewinding to
            return rewound_snapshot;
        }
    }
}
