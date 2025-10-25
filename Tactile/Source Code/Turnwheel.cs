using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TactileListExtension;
using System.Threading;

namespace Tactile
{
    class Turnwheel
    {
        private List<Turnwheel_Snapshot> Snapshots = new List<Turnwheel_Snapshot> { };
        public List<Turnwheel_Snapshot> snapshots { get { return Snapshots; } }
        public Turnwheel_Snapshot current_snapshot { get { return Snapshots.Last(); } }
        private bool Active;
        private int Charges;

        private Thread SnapshotThread;
        public bool snapshot_in_progress { get { return SnapshotThread != null && SnapshotThread.IsAlive; } }

        public bool active { get { return Active; } set { Active = value; } }
        public int charges { get { return Charges; } set { Charges = value; } }
        public bool can_rewind { get { return charges > 0 && active; } }
        #region: Serialization
        public void write(BinaryWriter writer)
        {
            write_turn_start_snapshots(writer); // Only save turn start snapshots to suspends to save time
            writer.Write(Active);
            writer.Write(Charges);
        }
        private void write_turn_start_snapshots(BinaryWriter writer)
        {
            List<Turnwheel_Snapshot> turn_start_snapshots = new List<Turnwheel_Snapshot> { };
            foreach (Turnwheel_Snapshot snapshot in Snapshots)
            {
                if (snapshot.name == "Turn Start")
                    turn_start_snapshots.Add(snapshot);
            }
            turn_start_snapshots.write(writer);
        }
        public void read(BinaryReader reader)
        {
            Snapshots.read(reader);
            Active = reader.ReadBoolean();
            Charges = reader.ReadInt32();
        }
        #endregion

        public Turnwheel() 
        {
            Active = false;
            Charges = 0;
        }
        public void Take_Snapshot(string snapshot_name)
        {
            if (Global.scene.scene_type == "Scene_Map_Unit_Editor") // Prevents unit editor from crashing
                return;

            string temp_filename = System.IO.Path.GetTempFileName();
            Create_Snapshot(temp_filename, snapshot_name);

            wait_for_snapshot_thread();
            SnapshotThread = new Thread(new ParameterizedThreadStart(Take_Snapshot_Worker));
            SnapshotThread.Start(temp_filename);
        }
        public void Take_Snapshot_Worker(Object data)
        {
            string temp_filename = (string)data;
            
            Save_Snapshot(temp_filename);
            Delete_Temp_File(temp_filename);
        }
        public void wait_for_snapshot_thread()
        {
            while (snapshot_in_progress)
                System.Threading.Thread.Sleep(2);
        }
        private void Create_Snapshot(string temp_filename, string snapshot_name)
        {
            Turnwheel_Snapshot snapshot = new Turnwheel_Snapshot(snapshot_name);
            snapshot.Get_Global_Variables();
            snapshot.index = snapshots.Count();
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
            Charges -= 1;
            Turnwheel_Snapshot rewound_snapshot = Snapshots[index];
            int number_of_snapshots_to_remove = Snapshots.Count - (index + 1);
            Snapshots.RemoveRange(index + 1, number_of_snapshots_to_remove); // Discard snapshots that take place after the point we're rewinding to
            return rewound_snapshot;
        }
    }
}
