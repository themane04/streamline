let isThrottled = false;
let scrollHandler = null;

export function onScrollBottom(dotnetHelper) {
    // Remove existing scroll handler to avoid duplicates
    if (scrollHandler) {
        window.removeEventListener('scroll', scrollHandler);
    }

    const loadMoviesIfNeeded = () => {
        if (isThrottled) return;

        const scrollPosition = window.innerHeight + window.scrollY;
        const bottomPosition = document.body.offsetHeight;

        // Trigger loading if the screen isn't filled
        if (scrollPosition >= bottomPosition - 200 || document.body.offsetHeight <= window.innerHeight) {
            isThrottled = true;

            dotnetHelper.invokeMethodAsync('LoadNextPage')
                .then(() => {
                    setTimeout(() => {
                        isThrottled = false;

                        // Check again to ensure no movies are missed
                        const newScrollPosition = window.innerHeight + window.scrollY;
                        const newBottomPosition = document.body.offsetHeight;

                        if (newScrollPosition >= newBottomPosition - 200 || document.body.offsetHeight <= window.innerHeight) {
                            loadMoviesIfNeeded();
                        }
                    }, 3000);
                })
                .catch(error => console.error("Error invoking LoadNextPage:", error));
        }
    };

    scrollHandler = loadMoviesIfNeeded;
    window.addEventListener('scroll', scrollHandler);
    loadMoviesIfNeeded();
}

export function disposeOnScrollBottom() {
    if (scrollHandler) {
        window.removeEventListener('scroll', scrollHandler);
        scrollHandler = null;
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