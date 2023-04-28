class ObserverHandler {

    static #observer = [];

    static addObserver = (component, observerTargetId) => {
        let observerInstance = new IntersectionObserver(entries => {
            component.invokeMethodAsync('OnIntersection');
        });
        let observedElement = document.getElementById(observerTargetId);
        if (observedElement === null || observedElement === undefined)
            throw new Error(`Cannot add observer. The observable target with Id: '${observerTargetId}' was not found!`);
        observerInstance.observe(observedElement);
        ObserverHandler.#observer.push({ observerTargetId, observerInstance, observedElement });
    };

    static removeObserver = (observerTargetId) => {
        let observedElement = document.getElementById(observerTargetId);
        if (observedElement === null || observedElement === undefined)
            throw new Error(`Cannot remove observer. The observable target with Id: '${observerTargetId}' was not found!`);
        let observerToStop = ObserverHandler.#observer.find((el) => el.observerTargetId === observerTargetId);
        try {
            observerToStop.observerInstance.disconnect();
        }
        catch
        {
            console.log(`Cannot disconnect observer with Id: '${observerTargetId}'.`);
        }
        ObserverHandler.#observer = ObserverHandler.#observer.filter((el) => el.observerTargetId !== observerTargetId);
    };
}

window.ObserverHandler = ObserverHandler;