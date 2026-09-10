window.gastaDate = {
    showPicker(el) {
        if (el && typeof el.showPicker === 'function') {
            try {
                el.showPicker();
            } catch (e) {
                // showPicker() can throw if not called from a direct user gesture in
                // some browsers — fall back to focusing it instead.
                el.focus();
            }
        } else if (el) {
            // Older browsers without showPicker() support.
            el.focus();
            el.click();
        }
    }
};
