/*using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class AuthorizationServiceManager : MonoBehaviour
{
    private static AuthorizationServiceManager source;
    public static AuthorizationServiceManager Instance 
    {
        get
        {
            source ??= new();
            return source;
        }
    }

    private static FirebaseAuth _auth;
    private static DatabaseReference _dbref;
    void Awake()
    {
        if (source != null)
        {
            Destroy(gameObject);
            return;
        }
        source = this;
        DontDestroyOnLoad(gameObject);
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(Task =>
        {
            var status = Task.Result;
            if (status != DependencyStatus.Available)
            {
                Debug.LogWarning("Database not available.");
                return;
            }
            _auth = FirebaseAuth.DefaultInstance;
            _dbref = FirebaseDatabase.DefaultInstance.RootReference;
        });
    }
    public async Awaitable<bool> CreateOrSigninAndWriteAsync(string email,string epass, string _displayName)
    {
        bool suceed = false;
        await _auth.CreateUserWithEmailAndPasswordAsync(email,epass).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogWarning("Register failed, potentially already in database.");

                return;
            }
            FirebaseUser user = task.Result.User;
            UserProfile newprofile = new()
            {
                DisplayName = _displayName
            };
            user.UpdateUserProfileAsync(newprofile);

            Debug.Log("User created: "+task.Result.User.DisplayName);
            WriteDownLastLogin(task.Result.User);
            suceed = true;
        });
        return suceed;
    }
    public async Awaitable<(bool,string)> SignInAsync(string email, string epass)
    {
        bool suceed = false;
        string outName = "";
        await _auth.SignInWithEmailAndPasswordAsync(email,epass).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogWarning("Canceled or failed to sign in.");
                return;
            }
            var user = task.Result.User;
            outName = user.DisplayName;
            WriteDownLastLogin(user);
            suceed = true;
        });
        return (suceed,outName);
    }
    private void WriteDownLastLogin(FirebaseUser user)
    {
        var userdata = new Dictionary<string, object>
        {
            {"email",user.Email??""},
            {"lastlogintime", DateTimeOffset.UtcNow.ToUnixTimeSeconds()},
        };

        _dbref.Child("users").Child(user.UserId).UpdateChildrenAsync(userdata).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogWarning("Failed to write. Canceled or faulted.");
                return;
            }
        });
    }
    public void SaveCounterData(GameplayData data)
    {
        if (_auth.CurrentUser == null || data == null)
        {
            return;
        }
        FirebaseUser user = _auth.CurrentUser;
        var userdata = new Dictionary<string, object>
        {
            {"lastedittime", DateTimeOffset.UtcNow.ToUnixTimeSeconds()},
            {"counterdata", data.Score},
        };

        _dbref.Child("users").Child(user.UserId).UpdateChildrenAsync(userdata).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogWarning("Failed to save. Canceled or faulted.");
                return;
            }

            Debug.Log("saved!");
        }); 

        var LeaderboardData = new Dictionary<string, object>
        {
            {"score",data.Score},
            {"name", user.DisplayName},
        };

        _dbref.Child("leaderboards").Child("global").UpdateChildrenAsync(LeaderboardData).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogWarning("Failed to save to leaderboard. Canceled or faulted.");
                return;
            }

            Debug.Log("saved lb!");
        });
    }
    public async Task<GameplayData> LoadGameplayDataAsync()
    {
        if (_auth.CurrentUser == null)
        {
            await CreateOrSigninAndWriteAsync("test@mail.com", "123456", "System Developer");
            await SignInAsync("test@mail.com", "123456");
        }
        ;
        GameplayData newData = new();
        await _dbref.Child("users").Child(_auth.CurrentUser.UserId).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogWarning("Failed to read. Canceled or faulted, or I have no glasses.");
                return;
            }

            if (!task.Result.Exists)
            {
                Debug.LogWarning("Failed to read. No new snapshot of minecraft. :(");
                return;
            }
            float? counterdata = task.Result.Child("counterdata").Value as float?;

            if (counterdata == null)
            {
                Debug.LogWarning("Failed to read. No new stocks data. :(");
                return;                
            }

            newData.Score = (float)counterdata;
        });
        return newData;
    }
}
*/