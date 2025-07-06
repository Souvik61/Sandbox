mergeInto(LibraryManager.library, {
    DownloadFile: function (filenamePtr, dataPtr, dataLength) {
        var filename = UTF8ToString(filenamePtr);
        var data = new Uint8Array(Module.HEAPU8.buffer, dataPtr, dataLength);

        var blob = new Blob([data], { type: "application/octet-stream" });
        var url = URL.createObjectURL(blob);

        var a = document.createElement("a");
        a.href = url;
        a.download = filename;
        a.click();

        URL.revokeObjectURL(url);
    }
});
