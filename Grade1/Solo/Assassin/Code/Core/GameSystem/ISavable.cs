namespace Code.Core.GameSystem
{
    public interface ISavable
    {
        public SaveIdSO SaveID { get; }
        public string GetSaveData();
        public void RestoreData(string loadedData);
    }
}