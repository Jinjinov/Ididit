// This is a JavaScript module that is loaded on demand. It can export any number of
// functions, and may import other JavaScript modules if required.

export function showPrompt(message) {
  return prompt(message, 'Type anything here');
}

export function saveAsFile(filename, bytesBase64) {
    var link = document.createElement('a');
    link.download = filename;
    link.href = "data:application/octet-stream;base64," + bytesBase64;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

export function getDimensions() {
    return {
        width: window.innerWidth,
        height: window.innerHeight
    };
}

export function setElementProperty(element, property, value) {
    element[property] = value;
}

export async function readDirectoryFiles() {
    const dirHandle = await showDirectoryPicker();
    var directory = {
        kind: dirHandle.kind,
        name: dirHandle.name,
        nodes: []
    };
    await readDirectoryEntry(dirHandle, directory);
    return directory;
};
export async function readDirectoryEntry(dirHandle, directory) {
    for await (const handle of dirHandle.values()) {
        if (handle.kind === "file") {
            const file = await handle.getFile();
            const text = await file.text();
            var fileContent = {
                kind: handle.kind,
                name: handle.name,
                text: text
            };
            directory.nodes.push(fileContent);
        }
        if (handle.kind === "directory") {
            var subDirectory = {
                kind: handle.kind,
                name: handle.name,
                nodes: []
            };
            directory.nodes.push(subDirectory);
            await readDirectoryEntry(handle, subDirectory);
        }
    }
}

export async function writeDirectoryFiles(nodes) {
    const dirHandle = await showDirectoryPicker();
    await writeDirectoryEntry(dirHandle, nodes);
};
export async function writeDirectoryEntry(dirHandle, nodes) {
    for (const node of nodes) {
        if (node.kind === "file") {
            const newFileHandle = await dirHandle.getFileHandle(node.name, { create: true });

            const writable = await newFileHandle.createWritable();
            await writable.write(node.text);
            await writable.close();
        }
        if (node.kind === "directory") {
            const newDirectoryHandle = await dirHandle.getDirectoryHandle(node.name, { create: true });
            
            await writeDirectoryEntry(newDirectoryHandle, node.nodes);
        }
    }
}