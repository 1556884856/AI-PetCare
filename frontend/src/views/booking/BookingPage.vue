<template>
  <div class="page">
    <h1 class="page-title">📅 在线预约</h1>

    <el-steps :active="step" align-center class="steps">
      <el-step title="选择服务" />
      <el-step title="选择宠物和时间" />
      <el-step title="确认提交" />
    </el-steps>

    <div class="step-content">
      <div v-if="step === 0" class="step-panel">
        <h2>选择洗护服务</h2>
        <div class="choose-list">
          <div
            class="choose-card"
            :class="{ active: selectedService?.id === s.id }"
            v-for="s in displayedServices"
            :key="s.id"
            @click="selectedService = s"
          >
            <div class="choose-icon">{{ s.petType === 'Cat' ? '🐈' : '🐕' }}</div>
            <h3>{{ s.name }}</h3>
            <p>{{ s.description }}</p>
            <div class="choose-meta">
              <span>⏱ {{ s.durationMinutes }}分钟</span>
              <span class="price">¥{{ s.price }}</span>
            </div>
          </div>
        </div>
        <div v-if="hasMoreServices" class="load-more">
          <el-button type="primary" link @click="showAllServices = true">加载更多服务 ↓</el-button>
        </div>
        <div class="step-actions right">
          <el-button type="primary" size="large" :disabled="!selectedService" @click="step = 1">
            下一步：选择宠物和时间
          </el-button>
        </div>
      </div>

      <div v-if="step === 1" class="step-panel">
        <h2>选择宠物</h2>
        <div class="choose-list mini">
          <div
            class="choose-card"
            :class="{ active: selectedPet?.id === p.id }"
            v-for="p in pets"
            :key="p.id"
            @click="selectedPet = p"
          >
            <div class="choose-icon">{{ p.type === 'Cat' ? '🐈' : '🐕' }}</div>
            <h3>{{ p.name }}</h3>
            <p>{{ p.breed }}</p>
          </div>
          <div class="choose-card add-card" @click="showAddPet = true">
            <div class="choose-icon">➕</div>
            <h3>添加新宠物</h3>
          </div>
        </div>

        <h2 style="margin-top: 32px;">选择时间</h2>
        <div class="datetime-row">
          <el-date-picker
            v-model="selectedDate"
            type="date"
            placeholder="选择日期"
            :disabled-date="disabledDate"
          />
          <el-select v-if="selectedDate" :model-value="selectedSlot" @update:model-value="selectedSlot = $event" placeholder="请选择时间段" size="large" style="width: 260px;">
            <el-option v-for="slot in timeSlots" :key="slot" :label="slot" :value="slot" />
          </el-select>
        </div>

        <div class="step-actions">
          <el-button size="large" @click="step = 0">上一步</el-button>
          <el-button type="primary" size="large" :disabled="!selectedPet || !selectedDate || !selectedSlot" @click="step = 2">
            下一步：确认预约
          </el-button>
        </div>
      </div>

      <div v-if="step === 2" class="step-panel confirm">
        <h2>📋 预约确认</h2>
        <div class="confirm-card">
          <div class="confirm-row"><span>服务项目</span><strong>{{ selectedService?.name }}</strong></div>
          <div class="confirm-row"><span>预约宠物</span><strong>{{ selectedPet?.type === 'Cat' ? '🐈' : '🐕' }} {{ selectedPet?.name }}</strong></div>
          <div class="confirm-row"><span>预约日期</span><strong>{{ selectedDate }}</strong></div>
          <div class="confirm-row"><span>预约时段</span><strong>{{ selectedSlot }}</strong></div>
          <div class="confirm-row"><span>预计金额</span><strong class="price">¥{{ selectedService?.price }}</strong></div>
        </div>

        <div class="coupon-section" v-if="myCoupons.length > 0">
          <h3>选择优惠券</h3>
          <el-select v-model="selectedCouponId" placeholder="不使用优惠券" clearable style="width: 100%;">
            <el-option
              v-for="c in myCoupons"
              :key="c.id"
              :label="couponLabel(c)"
              :value="c.id"
              :disabled="!isCouponValid(c)"
            />
          </el-select>
          <div v-if="selectedCouponId" class="coupon-discount">
            预计优惠：<strong>-¥{{ calcDiscount }}</strong>
          </div>
        </div>

        <div class="notes-input">
          <el-input v-model="notes" type="textarea" :rows="2" placeholder="备注（选填）：如狗狗怕水、有皮肤病等特殊情况..." />
        </div>
        <div class="step-actions">
          <el-button @click="step = 1">上一步</el-button>
          <el-button type="warning" size="large" :loading="submitting" @click="submitBooking">✓ 确认预约并支付</el-button>
        </div>
      </div>
    </div>

    <el-dialog v-model="showAddPet" title="添加宠物" width="420px">
      <el-form :model="newPet" label-position="top">
        <el-form-item label="宠物名字"><el-input v-model="newPet.name" /></el-form-item>
        <el-form-item label="类型">
          <el-radio-group v-model="newPet.type">
            <el-radio value="Dog">🐕 狗狗</el-radio>
            <el-radio value="Cat">🐈 猫咪</el-radio>
          </el-radio-group>
        </el-form-item>
        <el-form-item label="品种"><el-input v-model="newPet.breed" placeholder="如：金毛、英短" /></el-form-item>
        <el-form-item label="年龄（月）"><el-input-number v-model="newPet.age" :min="1" :step="1" /></el-form-item>
        <el-form-item label="体重（kg）"><el-input-number v-model="newPet.weight" :min="0.5" :step="0.5" /></el-form-item>
        <el-form-item label="备注"><el-input v-model="newPet.notes" placeholder="特殊情况说明" /></el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="showAddPet = false">取消</el-button>
        <el-button type="primary" @click="addPet">确认添加</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { getServices } from '@/api/services'
import { getMyPets, createPet } from '@/api/pets'
import { createAppointment } from '@/api/appointments'
import { getMyCoupons } from '@/api/coupons'
import { ElMessage } from 'element-plus'

const route = useRoute()
const router = useRouter()

const step = ref(0)
const services = ref<any[]>([])
const pets = ref<any[]>([])
const myCoupons = ref<any[]>([])
const selectedService = ref<any>(null)
const selectedPet = ref<any>(null)
const selectedDate = ref('')
const selectedSlot = ref('')
const selectedCouponId = ref<number | null>(null)
const notes = ref('')
const submitting = ref(false)
const showAddPet = ref(false)
const showAllServices = ref(false)

const displayedServices = computed(() => showAllServices.value ? services.value : services.value.slice(0, 9))
const hasMoreServices = computed(() => services.value.length > 9 && !showAllServices.value)

const timeSlots = ['09:00-10:00', '10:00-11:00', '11:00-12:00', '14:00-15:00', '15:00-16:00', '16:00-17:00', '17:00-18:00']

const newPet = ref({ name: '', type: 'Dog', breed: '', age: 12, weight: 10, notes: '' })

const disabledDate = (date: Date) => {
  const today = new Date()
  today.setHours(0, 0, 0, 0)
  return date.getTime() < today.getTime()
}

onMounted(async () => {
  const [sRes, pRes, cRes] = await Promise.all([
    getServices(),
    getMyPets().catch(() => ({ data: [] })),
    getMyCoupons('unused').catch(() => ({ data: [] }))
  ])
  services.value = (sRes as any).data || []
  pets.value = (pRes as any).data || []
  myCoupons.value = (cRes as any).data || []

  const serviceId = route.query.serviceId
  if (serviceId) {
    selectedService.value = services.value.find((s: any) => s.id === Number(serviceId))
  }
})

const addPet = async () => {
  try {
    const res: any = await createPet(newPet.value)
    pets.value.push(res.data)
    showAddPet.value = false
    newPet.value = { name: '', type: 'Dog', breed: '', age: 12, weight: 10, notes: '' }
    ElMessage.success('宠物添加成功')
  } catch {}
}

const isCouponValid = (c: any) => {
  if (!selectedService.value) return false
  return c.minOrderAmount <= selectedService.value.price
}

const couponLabel = (c: any) => {
  const val = c.type === 0 ? `¥${c.value}` : `${(c.value * 10).toFixed(1)}折`
  const cond = c.minOrderAmount > 0 ? `满¥${c.minOrderAmount}` : '无门槛'
  return `${c.couponName} (${val} ${cond})`
}

const calcDiscount = computed(() => {
  if (!selectedCouponId.value || !selectedService.value) return '0.00'
  const coupon = myCoupons.value.find(c => c.id === selectedCouponId.value)
  if (!coupon) return '0.00'
  if (coupon.type === 0) return coupon.value.toFixed(2)
  return (selectedService.value.price * (1 - coupon.value)).toFixed(2)
})

const submitBooking = async () => {
  submitting.value = true
  try {
    const res: any = await createAppointment({
      serviceId: selectedService.value.id,
      petId: selectedPet.value.id,
      appointmentDate: selectedDate.value,
      timeSlot: selectedSlot.value,
      notes: notes.value
    })
    const appointmentId = res.data.id
    ElMessage.success('预约成功，即将跳转到支付页面...')
    router.push({ path: '/payment', query: { appointmentId, couponId: selectedCouponId.value ?? undefined } })
  } catch {} finally {
    submitting.value = false
  }
}
</script>

<style scoped>
.page { max-width: 900px; margin: 0 auto; padding: 48px 24px; }
.page-title { font-size: 32px; text-align: center; margin-bottom: 32px; }
.steps { margin-bottom: 40px; }
.step-panel { padding: 32px 0; }
.step-panel h2 { font-size: 22px; margin-bottom: 20px; }
.choose-list { display: grid; grid-template-columns: repeat(3, 1fr); gap: 16px; }
.choose-list.mini { grid-template-columns: repeat(4, 1fr); }
.choose-card {
  background: #fff;
  border: 2px solid var(--border);
  border-radius: var(--radius-md);
  padding: 24px 16px;
  text-align: center;
  cursor: pointer;
  transition: all var(--transition);
}
.choose-card:hover { border-color: var(--primary); }
.choose-card.active { border-color: var(--primary); background: var(--primary-bg); }
.choose-icon { font-size: 36px; margin-bottom: 8px; }
.choose-card h3 { font-size: 15px; margin-bottom: 4px; }
.choose-card p { color: var(--muted); font-size: 13px; margin-bottom: 8px; }
.choose-meta { display: flex; justify-content: center; gap: 12px; font-size: 13px; }
.price { color: var(--secondary); font-weight: 700; }
.add-card { border-style: dashed; color: var(--muted); }
.datetime-row { display: flex; align-items: center; gap: 16px; margin-bottom: 32px; }
.slot-list { display: flex; flex-wrap: wrap; gap: 10px; margin-bottom: 32px; }
.step-actions { display: flex; justify-content: center; gap: 16px; margin-top: 48px; }
.step-actions.right { justify-content: flex-end; }
.confirm-card {
  background: #fff;
  border-radius: var(--radius-md);
  padding: 24px;
  box-shadow: var(--shadow-card);
}
.confirm-row { display: flex; justify-content: space-between; padding: 12px 0; border-bottom: 1px solid var(--border); }
.confirm-row:last-child { border-bottom: none; }
.confirm-row span { color: var(--muted); }
.load-more { text-align: center; margin-top: 20px; }
.notes-input { margin-top: 20px; }
.coupon-section { margin-top: 20px; }
.coupon-section h3 { font-size: 16px; margin-bottom: 10px; }
.coupon-discount { margin-top: 10px; font-size: 14px; color: var(--secondary); text-align: right; }
@media (max-width: 768px) { .choose-list { grid-template-columns: 1fr; } .choose-list.mini { grid-template-columns: repeat(2, 1fr); } }
</style>
