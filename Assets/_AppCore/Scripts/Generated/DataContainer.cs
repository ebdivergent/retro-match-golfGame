using AppCore;

public class DataContainer 
{
IDataManager _dataManager;

public DataContainer(IDataManager dataManager)
{
 _dataManager = dataManager;
}

public float MasterVolume
{
    get { return _dataManager.DefaultPrefsManager.GetFloat(PrefsKey.MasterVolume_d_float, 0.0f); }
    set { _dataManager.DefaultPrefsManager.SetFloat(PrefsKey.MasterVolume_d_float, value); }
}
public float MusicVolume
{
    get { return _dataManager.DefaultPrefsManager.GetFloat(PrefsKey.MusicVolume_d_float, 0.0f); }
    set { _dataManager.DefaultPrefsManager.SetFloat(PrefsKey.MusicVolume_d_float, value); }
}
public float VolumeSound
{
    get { return _dataManager.DefaultPrefsManager.GetFloat(PrefsKey.VolumeSound_d_float, 0.0f); }
    set { _dataManager.DefaultPrefsManager.SetFloat(PrefsKey.VolumeSound_d_float, value); }
}
public float VolumeMusic
{
    get { return _dataManager.DefaultPrefsManager.GetFloat(PrefsKey.VolumeMusic_d_float, 0.0f); }
    set { _dataManager.DefaultPrefsManager.SetFloat(PrefsKey.VolumeMusic_d_float, value); }
}
public int BestScore
{
    get { return _dataManager.DefaultPrefsManager.GetInt(PrefsKey.BestScore_d_int, 0); }
    set { _dataManager.DefaultPrefsManager.SetInt(PrefsKey.BestScore_d_int, value); }
}
}
