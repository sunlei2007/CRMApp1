export function getChkValue() {
    const tblChk = document.querySelectorAll(".tbl-chk");
    var arr = [];
    tblChk.forEach(item => {
        if (item.checked)
            arr.push(item.value);
    })
    return arr.join(",");
}
export function delRow() {
    const tblChk = document.querySelectorAll(".tbl-chk");
    tblChk.forEach(item => {
        if (item.checked) {
            var tr = item.parentNode.parentNode;
            //找到表格
            var tbody = tr.parentNode;
            //删除行
            tbody.removeChild(tr);
        }
    })
}
export function setSelect(state) {
    const tblChk = document.querySelectorAll(".tbl-chk");
    tblChk.forEach(item => {

        item.checked = state;
        
    })
}

export function init() {
    const chkAll = document.querySelector(".chk-all");
    chkAll.onclick = function () {
     
        setSelect(this.checked);
    }
   

}