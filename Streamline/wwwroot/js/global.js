let isThrottled = false;
let scrollListener = null;

export function initializeScrollListener(dotnetHelper) {
    if (scrollListener) {
        window.removeEventListener('scroll', scrollListener);
    }

    scrollListener = () => {
        if (isThrottled) return;

        const scrollPosition = window.innerHeight + window.scrollY;
        const bottomPosition = document.body.offsetHeight;

        if (scrollPosition >= bottomPosition - 200) {
            isThrottled = true;

            dotnetHelper.invokeMethodAsync('LoadNextPage')
                .then(() => {
                    setTimeout(() => {
                        isThrottled = false;
                        const newScrollPosition = window.innerHeight + window.scrollY;
                        const newBottomPosition = document.body.offsetHeight;

                        if (newScrollPosition >= newBottomPosition - 200) {
                            dotnetHelper.invokeMethodAsync('LoadNextPage').catch(console.error);
                        }
                    }, 3000);
                })
                .catch(error => console.error("Error invoking LoadNextPage:", error));
        }
    };

    window.addEventListener('scroll', scrollListener);
}

export function cleanupScrollListener() {
    if (scrollListener) {
        window.removeEventListener('scroll', scrollListener);
        scrollListener = null;
    }
}


export function triggerFileUpload(elementId) {
    const fileInput = document.getElementById(elementId);
    if (fileInput) {
        fileInput.click();
    } else {
        console.error(`Element with ID ${elementId} not found.`);
    }
}
