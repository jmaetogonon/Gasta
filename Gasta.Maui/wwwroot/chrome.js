// Host-only (MAUI) script: reports the page's background colour to the native shell so the
// status bar / navigation bar strips match the current theme and accent. Watches the same
// things the app changes: <html data-theme>, inline style, and the accent <style> tag.
window.gastaChrome = {
    start(ref) {
        function toHex(css) {
            const m = css.match(/[\d.]+/g);
            if (!m || m.length < 3) return null;
            if (m.length > 3 && Number(m[3]) === 0) return null; // transparent
            return '#' + m.slice(0, 3).map(x => Math.round(Number(x)).toString(16).padStart(2, '0')).join('');
        }

        let last = '';
        function push() {
            const hex = toHex(getComputedStyle(document.body).backgroundColor);
            if (!hex) return;
            const dark = document.documentElement.getAttribute('data-theme') === 'dark';
            const key = hex + '|' + dark;
            if (key === last) return;
            last = key;
            ref.invokeMethodAsync('OnPageColor', hex, dark);
        }

        // theme.css animates the background for 0.2s, so read it only once that has finished;
        // reading earlier would report the old colour together with the new theme flag.
        let timer = 0;
        function schedule() {
            clearTimeout(timer);
            timer = setTimeout(push, 260);
        }

        new MutationObserver(schedule).observe(document.documentElement,
            { attributes: true, attributeFilter: ['data-theme', 'style', 'class'] });
        new MutationObserver(schedule).observe(document.head,
            { childList: true, subtree: true, characterData: true });
        schedule();

        // Android's file picker greys out files whose stored type doesn't match the input's
        // accept=".json,.csv" (files saved through "Save to device" can carry a type it doesn't
        // map to). Settings validates the chosen file's content itself, so in the Android app the
        // picker is left unfiltered. The PWA is untouched: this script only exists in this host.
        function relaxFileAccept() {
            document.querySelectorAll('input[type=file][accept]').forEach(i => i.removeAttribute('accept'));
        }
        relaxFileAccept();
        new MutationObserver(relaxFileAccept).observe(document.body,
            { childList: true, subtree: true, attributes: true, attributeFilter: ['accept'] });
    }
};
