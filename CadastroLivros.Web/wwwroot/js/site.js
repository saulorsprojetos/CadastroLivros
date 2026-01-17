(function ()
    {
        function formatBRLFromDigits(digits) {
            digits = (digits || "").replace(/\D/g, "");
            if (digits.length === 0) return "";
            
            digits = digits.replace(/^0+(?=\d)/, "");

            if (digits.length === 1) digits = "0" + digits;
            if (digits.length === 2) digits = "0" + digits;

            var cents = digits.slice(-2);
            var ints = digits.slice(0, -2);

            ints = ints.replace(/\B(?=(\d{3})+(?!\d))/g, ".");

            return ints + "," + cents;
        }

        function maskMoney(el)
        {
            var formatted = formatBRLFromDigits(el.value);
            el.value = formatted;
        }

        function attachMoneyMask()
        {
            document.querySelectorAll("input.money-br").forEach(function (el) {
                if (el.dataset.moneyMask === "1") return;
                el.dataset.moneyMask = "1";

                el.addEventListener("input", function () {
                    var start = el.selectionStart;
                    var before = el.value.length;

                    maskMoney(el);

                    var after = el.value.length;
                    var diff = after - before;
                    el.setSelectionRange(Math.max(0, start + diff), Math.max(0, start + diff));
                });

                el.addEventListener("blur", function () {
                    if (!el.value) return;
                    maskMoney(el);
                });


                if (el.value) maskMoney(el);
            })
        }

        document.addEventListener("DOMContentLoaded", attachMoneyMask);

})();

(function () {
    function enablePtBrNumberValidation() {
        if (!window.jQuery || !jQuery.validator) return;
        
        jQuery.validator.messages.number = "Informe um valor numérico válido.";

        jQuery.validator.methods.number = function (value, element) {
            if (this.optional(element)) return true;
            if (!value) return true;

            value = value.trim();
            return /^-?\d{1,3}(\.\d{3})*(,\d+)?$/.test(value)
                || /^-?\d+(,\d+)?$/.test(value);
        };

        document.querySelectorAll('input[data-val-number]').forEach(function (el) {
            el.setAttribute('data-val-number', 'Informe um valor numérico válido.');
        });
    }

    document.addEventListener("DOMContentLoaded", enablePtBrNumberValidation);
})();
