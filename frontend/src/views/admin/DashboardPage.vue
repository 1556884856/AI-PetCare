<template>
  <div>
    <h1 class="admin-title">📊 仪表盘</h1>
    <div class="stat-grid">
      <div class="stat-card">
        <div class="stat-icon" style="background:#E6F7F5;">📅</div>
        <div class="stat-info">
          <div class="stat-value">{{ stats.todayAppointments }}</div>
          <div class="stat-label">今日预约</div>
        </div>
      </div>
      <div class="stat-card">
        <div class="stat-icon" style="background:#FEF3C7;">⏳</div>
        <div class="stat-info">
          <div class="stat-value">{{ stats.pendingAppointments }}</div>
          <div class="stat-label">待确认</div>
        </div>
      </div>
      <div class="stat-card">
        <div class="stat-icon" style="background:#E6F7E6;">💰</div>
        <div class="stat-info">
          <div class="stat-value">¥{{ stats.todayRevenue }}</div>
          <div class="stat-label">今日营收</div>
        </div>
      </div>
      <div class="stat-card">
        <div class="stat-icon" style="background:#EDE9FE;">👥</div>
        <div class="stat-info">
          <div class="stat-value">{{ stats.monthNewCustomers }}</div>
          <div class="stat-label">本月新客户</div>
        </div>
      </div>
    </div>

    <div class="table-section">
      <h3>今日预约列表</h3>
      <el-table :data="todayList" v-loading="loading" style="width: 100%">
        <el-table-column prop="timeSlot" label="时段" width="130" />
        <el-table-column label="客户" width="120">
          <template #default="{ row }">{{ row.customerName || row.customerPhone }}</template>
        </el-table-column>
        <el-table-column label="宠物" width="120">
          <template #default="{ row }">{{ row.petType === 'Cat' ? '🐈' : '🐕' }} {{ row.petName }}</template>
        </el-table-column>
        <el-table-column prop="serviceName" label="服务" />
        <el-table-column prop="price" label="金额" width="100">
          <template #default="{ row }">¥{{ row.price }}</template>
        </el-table-column>
        <el-table-column label="状态" width="100">
          <template #default="{ row }">
            <el-tag :type="statusType(row.status)">{{ statusLabel(row.status) }}</el-tag>
          </template>
        </el-table-column>
      </el-table>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { getDashboard } from '@/api/admin'

const stats = reactive({ todayAppointments: 0, pendingAppointments: 0, todayRevenue: 0, monthNewCustomers: 0 })
const todayList = ref<any[]>([])
const loading = ref(false)

const statusType = (s: number) => ({ 0: 'warning', 1: 'primary', 2: 'success', 3: 'info' } as any)[s]
const statusLabel = (s: number) => ({ 0: '待确认', 1: '已确认', 2: '已完成', 3: '已取消' } as any)[s]

onMounted(async () => {
  loading.value = true
  try {
    const res: any = await getDashboard()
    if (res.data) {
      Object.assign(stats, res.data.stats || {})
      todayList.value = res.data.todayAppointments || []
    }
  } catch {} finally { loading.value = false }
})
</script>

<style scoped>
.admin-title { font-size: 24px; margin-bottom: 24px; }
.stat-grid { display: grid; grid-template-columns: repeat(4, 1fr); gap: 20px; margin-bottom: 32px; }
.stat-card { background: #fff; border-radius: var(--radius-md); padding: 20px; display: flex; align-items: center; gap: 16px; box-shadow: var(--shadow-card); }
.stat-icon { font-size: 32px; width: 56px; height: 56px; display: flex; align-items: center; justify-content: center; border-radius: var(--radius-sm); }
.stat-value { font-size: 28px; font-weight: 700; }
.stat-label { color: var(--muted); font-size: 13px; }
.table-section { background: #fff; border-radius: var(--radius-md); padding: 24px; box-shadow: var(--shadow-card); }
.table-section h3 { font-size: 18px; margin-bottom: 16px; }
@media (max-width: 768px) { .stat-grid { grid-template-columns: repeat(2, 1fr); } }
</style>
