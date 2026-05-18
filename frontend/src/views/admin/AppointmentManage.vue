<template>
  <div>
    <div class="admin-header">
      <h1>📅 预约管理</h1>
      <div class="header-filters">
        <el-date-picker v-model="filterDate" type="date" placeholder="按日期筛选" @change="fetchList" />
        <el-select v-model="filterStatus" placeholder="状态" clearable @change="fetchList" style="width:120px">
          <el-option label="待确认" :value="0" /><el-option label="已确认" :value="1" />
          <el-option label="已完成" :value="2" /><el-option label="已取消" :value="3" />
        </el-select>
      </div>
    </div>
    <el-table :data="list" v-loading="loading" style="width: 100%">
      <el-table-column label="日期" width="120">
        <template #default="{ row }">{{ formatDate(row.appointmentDate) }}</template>
      </el-table-column>
      <el-table-column prop="timeSlot" label="时段" width="130" />
      <el-table-column label="客户" width="120"><template #default="{ row }">{{ row.customerName || row.customerPhone }}</template></el-table-column>
      <el-table-column label="宠物"><template #default="{ row }">{{ row.petType === 'Cat' ? '🐈' : '🐕' }} {{ row.petName }}</template></el-table-column>
      <el-table-column prop="serviceName" label="服务" />
      <el-table-column label="金额" width="100"><template #default="{ row }">¥{{ row.price }}</template></el-table-column>
      <el-table-column label="状态" width="100">
        <template #default="{ row }"><el-tag :type="sType(row.status)">{{ sLabel(row.status) }}</el-tag></template>
      </el-table-column>
      <el-table-column label="操作" width="240">
        <template #default="{ row }">
          <el-button v-if="row.status === 0" size="small" type="success" @click="confirmOne(row.id)">确认</el-button>
          <el-button v-if="row.status === 1" size="small" type="primary" @click="completeOne(row.id)">完成</el-button>
          <el-button v-if="row.status === 0 || row.status === 1" size="small" type="danger" @click="cancelOne(row.id)">取消</el-button>
        </template>
      </el-table-column>
    </el-table>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { getAllAppointments, confirmAppointment, completeAppointment, adminCancelAppointment } from '@/api/appointments'
import { ElMessage, ElMessageBox } from 'element-plus'

const list = ref<any[]>([])
const loading = ref(false)
const filterDate = ref('')
const filterStatus = ref<number | ''>('')
const sType = (s: number) => ({ 0: 'warning', 1: 'primary', 2: 'success', 3: 'info' } as any)[s]
const sLabel = (s: number) => ({ 0: '待确认', 1: '已确认', 2: '已完成', 3: '已取消' } as any)[s]

const fetchList = async () => {
  loading.value = true
  try {
    const params: any = {}
    if (filterDate.value) params.date = filterDate.value
    if (filterStatus.value !== '') params.status = filterStatus.value
    const res: any = await getAllAppointments(params)
    list.value = res.data || []
  } catch {} finally { loading.value = false }
}

const confirmOne = async (id: number) => { try { await confirmAppointment(id); ElMessage.success('已确认'); fetchList() } catch {} }
const completeOne = async (id: number) => { try { await completeAppointment(id); ElMessage.success('已完成'); fetchList() } catch {} }
const cancelOne = async (id: number) => {
  await ElMessageBox.confirm('确定取消？', '提示', { type: 'warning' })
  try { await adminCancelAppointment(id); ElMessage.success('已取消'); fetchList() } catch {}
}

const formatDate = (val: string) => val ? val.slice(0, 10) : ''
onMounted(fetchList)
</script>

<style scoped>
.admin-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 24px; flex-wrap: wrap; gap: 12px; }
.admin-header h1 { font-size: 24px; }
.header-filters { display: flex; gap: 12px; }
</style>
