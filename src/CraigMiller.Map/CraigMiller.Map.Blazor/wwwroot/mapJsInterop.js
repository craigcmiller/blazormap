export function fitCanvasToContainer(canvasId) {
    const canvas = document.getElementById(canvasId);

    canvas.style.width = '100%';
    canvas.style.height = '100%';

    canvas.width = canvas.offsetWidth;
    canvas.height = canvas.offsetHeight;
}

export function disableEventListener(elementId, listenerName) {
    const element = document.getElementById(elementId);
    if (element) {
        element.addEventListener(listenerName, e => e.preventDefault());
    }
}

export function getElementBoundingClientRect(elementId) {
    const element = document.getElementById(elementId);
    if (element) {
        return element.getBoundingClientRect();
    }

    return null;
}

export function getDevicePixelRatio() {
    return window.devicePixelRatio;
}
