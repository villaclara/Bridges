mergeInto(LibraryManager.library, {
    IsMobilePlatform: function () {
        var isMobile = /Android|iPhone|iPad|iPod|Mobile/i.test(navigator.userAgent);
        return isMobile ? 1 : 0;
    }
});