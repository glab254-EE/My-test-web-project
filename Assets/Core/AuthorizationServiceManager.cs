using System;
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
    public static AuthorizationServiceManager Instance {get
        {
            return source;
        }
    }

    private static FirebaseAuth _auth;
    private static DatabaseReference _dbref;
    private Dictionary<string,string> MailToKeysDictionary = new();
    private const string GLYPHS = "abcdefghijklmnopqrstuvwxyz0123456789"; 
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
                Debug.LogWarning("Failed to write throught email. Canceled or faulted.");
                return;
            }

            Debug.Log("wrotntndnn!");
        });
    }
    private void WriteDownLastLogin(FirebaseUser user,string DisplayName)
    {
        var userdata = new Dictionary<string, object>
        {
            {"email",user.Email??""},
            {"displayName",DisplayName},
            {"lastlogintime", DateTimeOffset.UtcNow.ToUnixTimeSeconds()},
        };

        _dbref.Child("users").Child(user.UserId).UpdateChildrenAsync(userdata).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogWarning("Failed to write throught email. Canceled or faulted.");
                return;
            }

            Debug.Log("wrotntndnn!");
        });
    }

}
