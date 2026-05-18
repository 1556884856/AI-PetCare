<template>
  <div class="me-page">
    <div class="me-layout">
      <aside class="me-nav">
        <div class="user-card">
          <el-avatar :size="56" icon="UserFilled" />
          <div><h3>{{ auth.user?.nickname || auth.user?.phone }}</h3><p>{{ auth.user?.phone }}</p></div>
        </div>
        <div class="nav-links">
          <router-link to="/me" class="nav-link">个人信息</router-link>
          <router-link to="/me/pets" class="nav-link">我的宠物</router-link>
          <router-link to="/me/appointments" class="nav-link active">我的预约</router-link>
        </div>
      </aside>
      <main class="me-content">
        <div class="content-header">
          <h2>📅 我的预约</h2>
          <el-radio-group v-model="filterStatus" @change="fetchList" size="small">
            <el-radio-button value="">全部</el-radio-button>
            <el-radio-button :value="0">待确认</el-radio-button>
            <el-radio-button :value="1">已确认</el-radio-button>
            <el-radio-button :value="2">已完成</el-radio-button>
            <el-radio-button :value="3">已取消</el-radio-button>
          </el-radio-group>
        </div>
        <el-table :data="list" v-loading="loading" style="width: 100%">
          <el-table-column label="日期" width="120">
            <template #default="{ row }">{{ formatDate(row.appointmentDate) }}</template>
          </el-table-column>
          <el-table-column prop="timeSlot" label="时段" width="130" />
          <el-table-column label="宠物" width="100">
            <template #default="{ row }">
              {{ row.petType === 'Cat' ? '🐈' : '🐕' }} {{ row.petName }}
            </template>
          </el-table-column>
          <el-table-column prop="serviceName" label="服务" />
          <el-table-column prop="price" label="金额" width="100">
            <template #default="{ row }">¥{{ row.price }}</template>
          </el-table-column>
          <el-table-column label="状态" width="100">
            <template #default="{ row }">
              <el-tag v-if="row.status === 0" type="warning">待确认</el-tag>
              <el-tag v-else-if="row.status === 1" type="primary">已确认</el-tag>
              <el-tag v-else-if="row.status === 2" type="success">已完成</el-tag>
              <el-tag v-else type="info">已取消</el-tag>
            </template>
          </el-table-column>
          <el-table-column label="操作" width="100">
            <template #default="{ row }">
              <el-button
                v-if="row.status === 0 || row.status === 1"
                size="small"
                type="danger"
                @click="cancelOne(row.id)"
              >取消</el-button>
            </template>
          </el-table-column>
        </el-table>
        <el-empty v-if="!loading && list.length === 0" description="暂无预约记录" />
      </main>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { getMyAppointments, cancelAppointment } from '@/api/appointments'
import { ElMessage, ElMessageBox } from 'element-plus'

const auth = useAuthStore()
const list = ref<any[]>([])
const loading = ref(false)
const filterStatus = ref('')

const fetchList = async () => {
  loading.value = true
  try {
    const params: any = {}
    if (filterStatus.value !== '') params.status = filterStatus.value
    const res: any = await getMyAppointments(params)
    list.value = res.data || []
  } catch {} finally { loading.value = false }
}

const cancelOne = async (id: number) => {
  await ElMessageBox.confirm('确定取消这个预约吗？', '提示', { type: 'warning' })
  try { await cancelAppointment(id); ElMessage.success('已取消'); fetchList() } catch {}
}

const formatDate = (val: string) => val ? val.slice(0, 10) : ''
onMounted(fetchList)
</script>

<style scoped>
.me-page { max-width: 1200px; margin: 0 auto; padding: 48px 24px; }
.me-layout { display: flex; gap: 32px; }
.me-nav { width: 220px; flex-shrink: 0; }
.user-card { display: flex; align-items: center; gap: 12px; padding: 20px; background: #fff; border-radius: var(--radius-md); box-shadow: var(--shadow-card); margin-bottom: 12px; }
.user-card h3 { font-size: 15px; }
.user-card p { color: var(--muted); font-size: 13px; }
.nav-links { background: #fff; border-radius: var(--radius-md); box-shadow: var(--shadow-card); overflow: hidden; }
.nav-link { display: block; padding: 14px 20px; font-size: 14px; border-left: 3px solid transparent; transition: all var(--transition); }
.nav-link:hover { background: var(--primary-bg); }
.nav-link.active { border-left-color: var(--primary); color: var(--primary); background: var(--primary-bg); font-weight: 600; }
.me-content { flex: 1; background: #fff; border-radius: var(--radius-md); padding: 32px; box-shadow: var(--shadow-card); }
.content-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 24px; flex-wrap: wrap; gap: 12px; }
.content-header h2 { font-size: 22px; }
@media (max-width: 768px) { .me-layout { flex-direction: column; } .me-nav { width: 100%; } }
</style>
