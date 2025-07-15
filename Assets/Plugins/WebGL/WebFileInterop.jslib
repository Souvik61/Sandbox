mergeInto(LibraryManager.library, {
    // Download file (save prompt)
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
    },

    UploadFile: function (gameObjectNamePtr, successCallbackPtr, cancelCallbackPtr, acceptPtr) {
    var gameObjectName = UTF8ToString(gameObjectNamePtr);
    var successCallback = UTF8ToString(successCallbackPtr);
    var cancelCallback = UTF8ToString(cancelCallbackPtr);
    var accept = UTF8ToString(acceptPtr);

    var input = document.createElement('input');
    input.type = 'file';
    input.accept = accept;
    input.style.display = 'none';

    document.body.appendChild(input);

    let cancelled = true;

    input.onchange = (event) => {
        if (input.files.length > 0) {
            cancelled = false;

            var file = input.files[0];
            var reader = new FileReader();

            reader.onload = () => {
				
				var result = {
                    filename: file.name,
                    content: reader.result
					};

                SendMessage(gameObjectName, successCallback, JSON.stringify(result));				
                document.body.removeChild(input);
            };

            reader.readAsText(file); // adjust if needed
        }
    };

    // Detect cancel by checking if input loses focus and no file selected
    input.onblur = () => {
        setTimeout(() => {
            if (cancelled) {
                SendMessage(gameObjectName, cancelCallback, "");
                document.body.removeChild(input);
            }
        }, 200);
    };

    input.click();
    input.focus();
}
});