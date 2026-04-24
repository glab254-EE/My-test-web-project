mergeInto(LibraryManager.library, {
    InitTelegram: function () {
        var str = "";
        if (typeof window !== "undefied"  && window.Telegram && window.Telegram.WebApp)
        {
            str = window.Telegram.WebApp.initData || "";
        } 
        var bufffSize = lengthBytesUTF8(str) + 1;
        var buffer = _malloc(bufffSize);
        stringToUTF8(str, buffer, bufffSize);
        
        return buffer;
    },
    GetTGUser: function() {
        var str = "{}";
        if (typeof window !== "undefied"  && window.Telegram && window.Telegram.WebApp && window.Telegram.WebApp.initDataUnsafe && window.Telegram.WebApp.initDataUnsafe.user)
        {
            var user = window.Telegram.WebApp.initDataUnsafe.user;
            str = JSON.stringify(user);
        } 
        var bufffSize = lengthBytesUTF8(str) + 1;
        var buffer = _malloc(bufffSize);
        stringToUTF8(str, buffer, bufffSize);

        return buffer;
    },
    SetupTelegram :function(){
        if (typeof window !== "undefied"  && window.Telegram && window.Telegram.WebApp)
        {
            window.Telegram.WebApp.ready();
        }
    },
    ExpandTelegram :function(){
        if (typeof window !== "undefied"  && window.Telegram && window.Telegram.WebApp)
        {
            window.Telegram.WebApp.expand();
        }
    },
})