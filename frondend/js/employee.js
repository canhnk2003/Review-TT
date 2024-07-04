window.onload = function () {
  new EmployeePage();
};

class EmployeePage {
  pageTitle = "Quản lý nhân viên";
  inputInvalids = [];
  constructor() {
    this.initEvents();
  }
  /**
   * Khởi tạo các sự kiện trong page
   */
  initEvents() {
    try {
      var me = this;
      //Click button Thêm mới hiển thị form thêm mới
      document
        .querySelector("#btnAdd")
        .addEventListener("click", this.btnAddOnClick);
      //Khi Click nút X (Close)
      const btnCloses = document.querySelectorAll(".m-dialog .m-dialog-close");
      for (const btnClose of btnCloses) {
        btnClose.addEventListener("click", function () {
          this.parentElement.parentElement.parentElement.style.visibility =
            "hidden";
            me.inputInvalids[0].focus();
        });
      }
      //Thêm focus vào ô input đầu tiên khi nhấn Đồng ý
      document
        .querySelector("#dlgNotice .m-button")
        .addEventListener("click", function () {
          this.parentElement.parentElement.parentElement.style.visibility =
            "hidden";
          me.inputInvalids[0].focus();
        });
      //Khi click nút Hủy trong dialog detail
      document
        .querySelector(".m-dialog .m-button-secondary")
        .addEventListener("click", () => {
          document.querySelector(".m-dialog").style.visibility = "hidden";
        });
      //Refresh dữ liệu
      //Khi nhấn nút Cất
      document
        .querySelector("#btnSave")
        .addEventListener("click", this.btnSaveOnClick.bind(this));
      //Xuất file Excel

      //Nhấn nút xóa thì hiển thị dialog xác nhận xóa
      const btnDeletes = document.querySelectorAll("table .m-edit");
      for (const btnDelete of btnDeletes) {
        btnDelete.addEventListener("click", this.btnDeleteOnClick);
      }
    } catch (error) {
      console.error(error);
    }
  }

  loadData() {}
  /**
   * Click vào button Thêm mới
   */
  btnAddOnClick() {
    try {
      //Hiển thị form Thêm mới
      //1. Lấy ra element của form thêm mới
      const dlgDetail = document.querySelector("#dlgDetail");
      //2. Set hiển thị form
      dlgDetail.style.visibility = "visible";
      dlgDetail.querySelector("#empId").focus();
    } catch (error) {
      console.error(error);
    }
  }
  btnSaveOnClick() {
    try {
      //1. Thực hiện kiểm tra dữ liệu hợp lệ
      const validateRequired = this.checkRequiredInput();
      //2. Đưa ra dialog thông báo nếu lỗi
      if (validateRequired.errors.length > 0) {
        let dlgNotice = document.querySelector("#dlgNotice");
        //Cho dialog hiện lên
        dlgNotice.style.visibility = "visible";
        //Đổi tên tiêu dề cho dialog
        dlgNotice.querySelector(".m-dialog-title").innerHTML =
          "Dữ liệu không hợp lệ";
        //Xóa hết nội dung cũ
        let errorElement = dlgNotice.querySelector(".m-dialog-row");
        errorElement.innerHTML = "";
        //Thêm thông báo lỗi
        errorElement.style.display = "block";
        for (const msg of validateRequired.errors) {
          const m = msg.replace(/\*/g, "");
          let li = document.createElement("li");
          li.textContent = m;
          errorElement.append(li);
        }
        //Focus vào ô input lỗi đầu tiên
        this.inputInvalids=validateRequired.inputInvalid;
      } else {
        //
      }
    } catch (error) {
      console.error(error);
    }
  }
  addErrorElementToInputInvalid(input) {
    try {
      //Thêm border màu đỏ
      input.classList.add("m-input-error");
      //Bổ sung thông tin lỗi
      let errorElementMsg = document.createElement("div");
      errorElementMsg.classList.add("m-input-error-msg");
      errorElementMsg.textContent = "Thông tin này không được để trống!";
      input.after(errorElementMsg);
    } catch (error) {
      console.log(error);
    }
  }
  checkRequiredInput() {
    try {
      let result = {
        inputInvalid: [],
        errors: [],
      };
      //Lấy ra các input bắt buộc nhập
      let inputs = document.querySelectorAll("#dlgDetail input[required]");
      for (const input of inputs) {
        const value = input.value;
        if (value === "" || value === null || value === undefined) {
          const lable = input.previousElementSibling;
          this.addErrorElementToInputInvalid(input);
          result.inputInvalid.push(input);
          result.errors.push(`${lable.textContent} không được để trống!`);
        } else {
          input.classList.remove("m-input-error");
          input.nextElementSibling.remove();
        }
      }
      return result;
    } catch (error) {
      console.log(error);
    }
  }
  btnDeleteOnClick(){
    try {
      let dlgDelete = document.querySelector("#dlgNotice");
      //Hiện ô dialog
      dlgDelete.style.visibility="visible";
      //Đổi tên tiêu đề dialog
      dlgDelete.querySelector(".m-dialog-title").innerHTML="Xác nhận xóa";
      //Lấy mã nhân viên
      // let listId = empIdList();
      dlgDelete.querySelector(".m-dialog-row").innerHTML="Bạn có chắc chắn muốn xóa ";
    } catch (error) {
      console.log(error);
    }
  }
  empIdList(){
    try {
      let listEmpId=[];
      const empIds=document.querySelectorAll("table [empIdCell]");
      for (const empId of empIds) {
        listEmpId.push(empId.textContent);
      }
      return listEmpId;
    } catch (error) {
      console.log(error);
    }
  }
}
