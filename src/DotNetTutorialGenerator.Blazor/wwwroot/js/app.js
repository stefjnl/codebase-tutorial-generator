// Initialize Mermaid.js diagrams
window.initializeMermaid = (elementId) => {
    if (typeof mermaid !== 'undefined') {
        // Configure mermaid
        mermaid.initialize({
            startOnLoad: true,
            theme: 'default',
            securityLevel: 'loose'
        });

        // Render the diagram
        mermaid.init(undefined, `#${elementId} .mermaid`);
    }
};

// Highlight code with Prism.js
window.highlightCode = () => {
    if (typeof Prism !== 'undefined') {
        Prism.highlightAll();
    }
};

// File drop zone functionality
window.initializeDropZone = (elementId, componentRef) => {
    const dropZone = document.getElementById(elementId);

    if (dropZone) {
        dropZone.addEventListener('dragover', (e) => {
            e.preventDefault();
            dropZone.classList.add('drag-over');
        });

        dropZone.addEventListener('dragleave', (e) => {
            e.preventDefault();
            dropZone.classList.remove('drag-over');
        });

        dropZone.addEventListener('drop', (e) => {
            e.preventDefault();
            dropZone.classList.remove('drag-over');

            if (e.dataTransfer.files.length > 0) {
                // In a real implementation, this would pass the files to the component
                componentRef.invokeMethodAsync('HandleDroppedFiles', e.dataTransfer.files.length);
            }
        });
    }
};

// Split layout resizing functionality
window.initializeSplitLayout = (splitterId, leftPaneId, rightPaneId, componentRef) => {
    const splitter = document.getElementById(splitterId);
    const leftPane = document.getElementById(leftPaneId);
    const rightPane = document.getElementById(rightPaneId);

    if (splitter && leftPane && rightPane) {
        let isResizing = false;

        splitter.addEventListener('mousedown', (e) => {
            isResizing = true;
            document.body.style.cursor = 'col-resize';
            e.preventDefault();
        });

        document.addEventListener('mousemove', (e) => {
            if (isResizing) {
                const container = splitter.parentElement;
                const containerRect = container.getBoundingClientRect();
                const relativeX = ((e.clientX - containerRect.left) / containerRect.width) * 100;

                if (relativeX > 10 && relativeX < 90) {
                    leftPane.style.width = `${relativeX}%`;
                    rightPane.style.width = `${100 - relativeX}%`;
                }
            }
        });

        document.addEventListener('mouseup', () => {
            if (isResizing) {
                isResizing = false;
                document.body.style.cursor = 'default';
            }
        });
    }
};

// Keyboard shortcuts
window.initializeKeyboardShortcuts = (componentRef) => {
    document.addEventListener('keydown', (e) => {
        // Ctrl+G for generate
        if (e.ctrlKey && e.key === 'g') {
            e.preventDefault();
            componentRef.invokeMethodAsync('HandleGenerateShortcut');
        }

        // Ctrl+H for history
        if (e.ctrlKey && e.key === 'h') {
            e.preventDefault();
            componentRef.invokeMethodAsync('HandleHistoryShortcut');
        }
    });
};

// Click element
window.clickElement = (element) => {
    element.click();
};
