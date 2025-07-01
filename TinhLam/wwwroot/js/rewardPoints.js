// Script xử lý điểm thưởng
class RewardPointsManager {
  constructor() {
    this.userId = null;
    this.currentPoints = 0;
    this.orderId = null;
    this.totalAmount = 0;
  }

  // Khởi tạo
  init(userId, orderId, totalAmount) {
    this.userId = userId;
    this.orderId = orderId;
    this.totalAmount = totalAmount;
    this.loadUserPoints();
  }

  // Load số điểm hiện tại của user
  async loadUserPoints() {
    try {
      const response = await fetch(`/RewardPoints/UserPoints/${this.userId}`);
      const data = await response.json();
      this.currentPoints = data.points;
      this.updatePointsDisplay();
    } catch (error) {
      console.error("Lỗi khi load điểm:", error);
    }
  }

  // Cập nhật hiển thị điểm
  updatePointsDisplay() {
    const pointsElement = document.getElementById("currentPoints");
    if (pointsElement) {
      pointsElement.textContent = this.currentPoints;
    }

    const pointsValueElement = document.getElementById("pointsValue");
    if (pointsValueElement) {
      pointsValueElement.textContent =
        (this.currentPoints * 1000).toLocaleString() + " VNĐ";
    }
  }

  // Sử dụng điểm thưởng
  async usePoints(points) {
    if (points > this.currentPoints) {
      alert("Số điểm không đủ!");
      return false;
    }

    if (points <= 0) {
      alert("Vui lòng nhập số điểm hợp lệ!");
      return false;
    }

    try {
      const response = await fetch("/RewardPoints/UsePoints", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          userId: this.userId,
          points: points,
          orderId: this.orderId,
        }),
      });

      const result = await response.json();

      if (result.success) {
        this.currentPoints -= points;
        this.updatePointsDisplay();
        this.updateOrderTotal(result.discountAmount);
        alert(
          `Đã sử dụng ${points} điểm để giảm ${result.discountAmount.toLocaleString()} VNĐ`
        );
        return true;
      } else {
        alert(result.message || "Có lỗi xảy ra!");
        return false;
      }
    } catch (error) {
      console.error("Lỗi khi sử dụng điểm:", error);
      alert("Có lỗi xảy ra khi sử dụng điểm!");
      return false;
    }
  }

  // Cập nhật tổng tiền đơn hàng
  updateOrderTotal(discountAmount) {
    const totalElement = document.getElementById("orderTotal");
    if (totalElement) {
      const currentTotal = parseFloat(
        totalElement.dataset.originalTotal || this.totalAmount
      );
      const newTotal = currentTotal - discountAmount;
      totalElement.textContent = newTotal.toLocaleString() + " VNĐ";
      totalElement.dataset.currentTotal = newTotal;
    }

    // Cập nhật input hidden cho form
    const totalInput = document.getElementById("totalAmount");
    if (totalInput) {
      const currentTotal = parseFloat(totalInput.value);
      totalInput.value = (currentTotal - discountAmount).toString();
    }
  }

  // Tính toán số tiền giảm giá từ điểm
  calculateDiscount(points) {
    return points * 1000;
  }

  // Tính toán số điểm cần thiết cho một số tiền giảm giá
  calculateRequiredPoints(discountAmount) {
    return Math.ceil(discountAmount / 1000);
  }
}

// Khởi tạo global instance
window.rewardPointsManager = new RewardPointsManager();

// Event handlers
document.addEventListener("DOMContentLoaded", function () {
  // Xử lý form sử dụng điểm
  const usePointsForm = document.getElementById("usePointsForm");
  if (usePointsForm) {
    usePointsForm.addEventListener("submit", async function (e) {
      e.preventDefault();

      const pointsInput = document.getElementById("pointsToUse");
      const points = parseInt(pointsInput.value);

      if (isNaN(points) || points <= 0) {
        alert("Vui lòng nhập số điểm hợp lệ!");
        return;
      }

      const success = await window.rewardPointsManager.usePoints(points);
      if (success) {
        pointsInput.value = "";
      }
    });
  }

  // Xử lý nút sử dụng tất cả điểm
  const useAllPointsBtn = document.getElementById("useAllPoints");
  if (useAllPointsBtn) {
    useAllPointsBtn.addEventListener("click", async function () {
      const points = window.rewardPointsManager.currentPoints;
      if (points > 0) {
        const success = await window.rewardPointsManager.usePoints(points);
        if (success) {
          document.getElementById("pointsToUse").value = "";
        }
      } else {
        alert("Bạn không có điểm nào để sử dụng!");
      }
    });
  }

  // Xử lý input số điểm để hiển thị preview
  const pointsInput = document.getElementById("pointsToUse");
  if (pointsInput) {
    pointsInput.addEventListener("input", function () {
      const points = parseInt(this.value) || 0;
      const discount = window.rewardPointsManager.calculateDiscount(points);
      const previewElement = document.getElementById("discountPreview");
      if (previewElement) {
        previewElement.textContent = `Giảm giá: ${discount.toLocaleString()} VNĐ`;
      }
    });
  }
});
